using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiCoreApi.Models.DbModels
{
    [Table("rbac_group_sync")]
    public class RbacGroupSyncModel
    {
        [Key] public int RbacGroupSyncId { get; set; }
        public string RbacGroupName { get; set; } = string.Empty;
        public string AiCoreGroupName { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}