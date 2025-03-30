using AiCoreApi.Common;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using static AiCoreApi.Common.ExceptionHandlingMiddleware;

namespace AiCoreApi.Services.ControllersServices;

public class EntraService : IEntraService
{
    private readonly ISettingsProcessor _settingsProcessor;
    private readonly IEntraTokenProvider _entraTokenProvider;
    private readonly ILogger<EntraService> _logger;

    public EntraService(
        ISettingsProcessor settingsProcessor,
        IEntraTokenProvider entraTokenProvider,
        ILogger<EntraService> logger)
    {
        _settingsProcessor = settingsProcessor;
        _entraTokenProvider = entraTokenProvider;
        _logger = logger;
    }

    public async Task<EntraCredentialExtendedItem> AddEntraCredential(EntraCredentialExtendedItem entraCredentialExtendedItem)
    {
        try
        {
            await _entraTokenProvider.SetCredentialsToKeyVaultAsync(entraCredentialExtendedItem.Name, entraCredentialExtendedItem.TenantId, entraCredentialExtendedItem.ClientId, entraCredentialExtendedItem.ClientSecret);
            var entraCredentials = _settingsProcessor.Get(SettingType.EntraCredentials); // id, name
            entraCredentialExtendedItem.EntraCredentialId = entraCredentials.Select(e => Convert.ToInt32(e.Key)).DefaultIfEmpty().Max() + 1;
            entraCredentials.Add(entraCredentialExtendedItem.EntraCredentialId.ToString(), entraCredentialExtendedItem.Name);
            _settingsProcessor.Set(SettingType.EntraCredentials, entraCredentials);
            return entraCredentialExtendedItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add Entra credential");
            throw new AiCoreUiException("Failed to add Entra credential");
        }
    }

    public async Task<List<EntraCredentialViewModel>> ListEntraCredentials()
    {
        var entraCredentials = _settingsProcessor.Get(SettingType.EntraCredentials);
        var entraCredentialViewModels = entraCredentials.Select(e => new EntraCredentialViewModel
        {
            EntraCredentialId = Convert.ToInt32(e.Key),
            Name = e.Value
        }).ToList();
        return entraCredentialViewModels;
    }

    public async Task DeleteEntraCredential(int entraCredentialId)
    {
        try
        {
            var entraCredentials = _settingsProcessor.Get(SettingType.EntraCredentials);
            var entraCredentialName = entraCredentials[entraCredentialId.ToString()];
            await _entraTokenProvider.RemoveCredentialsToKeyVaultAsync(entraCredentialName);
            entraCredentials.Remove(entraCredentialId.ToString());
            _settingsProcessor.Set(SettingType.EntraCredentials, entraCredentials);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Entra credential");
            throw new AiCoreUiException("Failed to delete Entra credential");
        }
    }
}

public interface IEntraService
{
    Task<EntraCredentialExtendedItem> AddEntraCredential(EntraCredentialExtendedItem entraCredentialExtendedItem);
    Task<List<EntraCredentialViewModel>> ListEntraCredentials();
    Task DeleteEntraCredential(int entraCredentialId);
}