namespace AiCore.FileIngestion.Service.Models.ViewModels
{
    public class EmbeddingConnectionModel
    {
        public string Endpoint { get; set; } = "";
        public string ModelName { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public string MaxTokens { get; set; } = "";
        public string IndexName { get; set; } = "";
        public ConnectionTypeEnum ConnectionType { get; set; } = ConnectionTypeEnum.Qdrant;
        public string ConnectionString { get; set; } = "";
    }

    public enum ConnectionTypeEnum
    {
        AzureAiSearch = 1,
        Qdrant = 2
    }
}
