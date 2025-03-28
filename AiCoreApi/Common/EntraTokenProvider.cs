using Azure.Core;
using Azure.Identity;
using System.Collections.Concurrent;
using Azure.Security.KeyVault.Secrets;

namespace AiCoreApi.Common
{
    public class EntraTokenProvider : IEntraTokenProvider
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();
        private static readonly ConcurrentDictionary<string, AccessToken> CachedTokens = new();
        private static readonly TimeSpan TokenRefreshBuffer = TimeSpan.FromMinutes(5);
        private static readonly DefaultAzureCredential DefaultCredential = new();
        public const string DefaultStorageName = "defaultManagedIdentity";
        private readonly ExtendedConfig _extendedConfig;

        public EntraTokenProvider(ExtendedConfig extendedConfig)
        {
            _extendedConfig = extendedConfig;
        }

        public async Task<string> GetAccessTokenAsync(string storageName, string resource) => (await GetAccessTokenObjectAsync(storageName, resource)).Token;

        public async Task<AccessToken> GetAccessTokenObjectAsync(string storageName, string resource)
        {
            var cacheKey = $"{storageName}|{resource}";
            if (CachedTokens.TryGetValue(cacheKey, out var token) && IsTokenValid(token))
                return token;

            var cacheLock = await GetLockAsync(cacheKey);
            try
            {
                await cacheLock.WaitAsync();
                // Double check again inside the lock
                if (CachedTokens.TryGetValue(cacheKey, out token) && IsTokenValid(token))
                    return token;

                AccessToken? newToken;
                var tokenRequestContext = new TokenRequestContext(new[] { resource });
                if (storageName == DefaultStorageName)
                {
                    newToken = await DefaultCredential.GetTokenAsync(tokenRequestContext);
                }
                else
                {
                    var credentials = await GetCredentialsFromKeyVaultAsync(storageName);
                    var credential = new ClientSecretCredential(credentials.TenantId, credentials.ClientId, credentials.ClientSecret);
                    newToken = await credential.GetTokenAsync(tokenRequestContext);
                }
                CachedTokens[cacheKey] = newToken.Value;
                return newToken.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get access token for: {storageName}", ex);
            }
            finally
            {
                cacheLock.Release();
            }
        }

        public async Task SetCredentialsToKeyVaultAsync(string storageName, string tenantId, string clientId, string clientSecret)
        {
            var client = GetSecretClient();
            var secretValue = $"{tenantId}|{clientId}|{clientSecret}";
            await client.SetSecretAsync(new KeyVaultSecret(storageName, secretValue));
        }

        public async Task RemoveCredentialsToKeyVaultAsync(string storageName)
        {
            var client = GetSecretClient();
            await client.StartDeleteSecretAsync(storageName);
        }

        private SecretClient GetSecretClient()
        {
            if (string.IsNullOrEmpty(_extendedConfig.KeyVaultUrl))
                throw new InvalidOperationException("Key Vault URL is not set in the configuration");
            if (!string.IsNullOrEmpty(_extendedConfig.KeyVaultAppClientId) &&
                !string.IsNullOrEmpty(_extendedConfig.KeyVaultAppClientSecret) &&
                !string.IsNullOrEmpty(_extendedConfig.KeyVaultAppTenantId))
            {
                return new SecretClient(new Uri(_extendedConfig.KeyVaultUrl),
                    new ClientSecretCredential(_extendedConfig.KeyVaultAppTenantId, _extendedConfig.KeyVaultAppClientId, _extendedConfig.KeyVaultAppClientSecret));
            }
            return new SecretClient(new Uri(_extendedConfig.KeyVaultUrl), DefaultCredential);
        }

        private async Task<SemaphoreSlim> GetLockAsync(string cacheKey) => Locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

        private async Task<ClientCredentials> GetCredentialsFromKeyVaultAsync(string storageName)
        {
            var client = GetSecretClient();
            var secretBundle = await client.GetSecretAsync(storageName);
            var secretValue = secretBundle.Value.Value;
            var secretParts = secretValue.Split('|');
            if (secretParts.Length != 3)
                throw new InvalidOperationException($"Invalid credentials format in Key Vault for: {storageName}");

            return new ClientCredentials
            {
                TenantId = secretParts[0],
                ClientId = secretParts[1],
                ClientSecret = secretParts[2]
            };
        }

        private bool IsTokenValid(AccessToken cachedToken) =>
            !string.IsNullOrEmpty(cachedToken.Token) &&
            DateTimeOffset.UtcNow < cachedToken.ExpiresOn.Subtract(TokenRefreshBuffer);

        private class ClientCredentials
        {
            public string TenantId { get; set; } = string.Empty;
            public string ClientId { get; set; } = string.Empty;
            public string ClientSecret { get; set; } = string.Empty;
        }
    }

    public class StaticTokenCredential: TokenCredential
    {
        private readonly AccessToken _accessToken;
        public StaticTokenCredential(string token, DateTimeOffset expiresOn)
        {
            _accessToken = new AccessToken(token, expiresOn);
        }
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return _accessToken;
        }
        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(_accessToken);
        }
    }


    public interface IEntraTokenProvider
    {
        Task<string> GetAccessTokenAsync(string storageName, string resource);
        Task<AccessToken> GetAccessTokenObjectAsync(string storageName, string resource);
        Task SetCredentialsToKeyVaultAsync(string storageName, string tenantId, string clientId, string clientSecret);
        Task RemoveCredentialsToKeyVaultAsync(string storageName);
    }
}
