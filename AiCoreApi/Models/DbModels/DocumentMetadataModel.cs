using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AiCoreApi.Models.DbModels
{
    [Table("document_metadata")]
    public class DocumentMetadataModel
    {
        [Key] public string DocumentId { get; private set; }
        public int IngestionId { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public DateTime LastMetadataUpdateTime { get; set; } = DateTime.UtcNow;
        public bool ImportFinished { get; set; }

        public DocumentMetadataModel(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(documentId));

            DocumentId = documentId;
        }
    }
}
