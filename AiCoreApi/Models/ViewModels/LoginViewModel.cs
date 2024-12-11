namespace AiCoreApi.Models.ViewModels
{
    public class LoginViewModel
    {
        public int LoginId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? LoginType { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public List<TagViewModel> Tags { get; set; } = new();
        public List<GroupViewModel> Groups { get; set; } = new();
        public int TokensLimit { get; set; } = 0;
    }
}
