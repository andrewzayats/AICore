namespace AiCoreApi.Models.ViewModels;

public class RbacRoleSyncViewModel
{
    public int RbacRoleSyncId { get; set; } = 0;
    public string RbacRoleName { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public List<TagViewModel> Tags { get; set; } = new();
}