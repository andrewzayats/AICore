namespace AiCoreApi.Models.ViewModels;

public class TagViewModel
{
    public int TagId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public List<GroupViewModel> Groups { get; set; } = new();
    public List<LoginViewModel> Logins { get; set; } = new();
    public List<IngestionViewModel> Ingestions { get; set; } = new();
}