using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiCoreApi.Models.DbModels
{
    [Table("rbac_role_sync")]
    public class RbacRoleSyncModel
    {
        [Key] public int RbacRoleSyncId { get; set; }
        public string RbacRoleName { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public List<TagModel> Tags { get; set; } = new();
    }
}