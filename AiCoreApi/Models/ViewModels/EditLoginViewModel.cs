namespace AiCoreApi.Models.ViewModels;

public class EditLoginViewModel
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool IsEnabled { get; set; }

    public List<TagViewModel> Tags { get; set; } = new();
    public List<GroupViewModel> Groups { get; set; } = new();
    public int TokensLimit { get; set; }
}