namespace AiCoreApi.Models.ViewModels
{
    public class EntraCredentialViewModel
    {
        public int EntraCredentialId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
    }

    public class EntraCredentialExtendedItem : EntraCredentialViewModel
    {
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
