using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AiCoreApi.Models.DbModels
{
    [Table("groups")]
    public class GroupModel
    {
        [JsonIgnore]
        [Key]
        public int GroupId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;

        public List<TagModel> Tags { get; set; } = new();
        public List<LoginModel> Logins { get; set; } = new();
        public List<ClientSsoModel> ClientSso { get; set; } = new();
        
    }
}