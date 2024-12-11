namespace AiCoreApi.Models.ViewModels;

public class GroupViewModel
{
    public int GroupId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public List<TagViewModel> Tags { get; set; } = new();
    public List<LoginViewModel> Logins { get; set; } = new();
}