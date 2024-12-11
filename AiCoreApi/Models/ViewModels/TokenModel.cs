using Newtonsoft.Json;

namespace AiCoreApi.Models.ViewModels
{
    public class TokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
        public string? RefreshToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "Bearer";
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; } = string.Empty;
        [JsonProperty("id_token", NullValueHandling = NullValueHandling.Ignore)]
        public string? IdToken { get; set; }
    }
}
