namespace AiCoreApi.Models.ViewModels
{
    public class ImportAgentsResultModel
    {
        public bool IsSuccess { get; set; } = true;
        public string ConfirmationId { get; set; } = string.Empty;
        public List<string> ConfirmationText { get; set; } = new();
    }
}
