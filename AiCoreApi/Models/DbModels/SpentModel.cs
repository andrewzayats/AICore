using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiCoreApi.Models.DbModels
{
    [Table("spent")]
    public class SpentModel
    {
        [Key] public int SpentId { get; set; }
        public int LoginId { get; set; } = 0;
        public string ModelName { get; set; } = string.Empty; 
        public int TokensOutgoing { get; set; } = 0;
        public int TokensIncoming { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    }
}