namespace AiCore.FileIngestion.Service.Models.ViewModels
{
    public record UploadFileRequestModel
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public byte[] Content { get; init; } = Array.Empty<byte>();
        public Dictionary<string, List<string>> Tags { get; init; } = new();
        public EmbeddingConnectionModel EmbeddingConnection { get; init; } = new();
        public TranslateStepModel TranslateStep { get; init; } = new();
    }

    public record TranslateStepModel
    {
        public string TargetLanguage { get; init; } = string.Empty;
        public string ApiKey { get; init; } = string.Empty;
        public string Region { get; init; } = string.Empty;
        public bool Enabled { get; init; } = false;
    }
}
