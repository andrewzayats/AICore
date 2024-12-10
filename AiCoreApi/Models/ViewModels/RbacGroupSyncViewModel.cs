namespace AiCoreApi.Models.ViewModels;

public class RbacGroupSyncViewModel
{
    public int RbacGroupSyncId { get; set; } = 0;
    public string RbacGroupName { get; set; } = string.Empty;
    public string AiCoreGroupName { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}