using AiCore.FileIngestion.Service.Common.DataFormats.Office;
using AiCore.FileIngestion.Service.Models.ViewModels;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DocumentStorage.DevTools;

namespace AiCore.FileIngestion.Service.Common
{
    public sealed class KernelMemoryProvider : IKernelMemoryProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Config _config;

        public KernelMemoryProvider(
            IHttpClientFactory httpClientFactory,
            Config config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public MemoryServerless GetKernelMemory(EmbeddingConnectionModel embeddingConnection)
        {
            var simpleFileStorageConfig = new SimpleFileStorageConfig
            {
                Directory = "_files",
                StorageType = Microsoft.KernelMemory.FileSystem.DevTools.FileSystemTypes.Disk
            };
            var httpClient = _httpClientFactory.CreateClient("RetryClient");
            var azureOpenAiEmbeddingConfig = new AzureOpenAIConfig
            {
                Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                APIKey = embeddingConnection.ApiKey,
                Endpoint = embeddingConnection.Endpoint,
                Deployment = embeddingConnection.ModelName,
                MaxTokenTotal = Convert.ToInt32(embeddingConnection.MaxTokens),
                APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                MaxRetries = 10,
            };
            var memoryBuilder = new KernelMemoryBuilder()
                .WithAzureOpenAITextEmbeddingGeneration(azureOpenAiEmbeddingConfig, httpClient: httpClient);
            if (embeddingConnection.ConnectionType == ConnectionTypeEnum.Qdrant)
            {
                var qdConfig = new QdrantConfig { Endpoint = embeddingConnection.ConnectionString };
                memoryBuilder = memoryBuilder.WithQdrantMemoryDb(qdConfig);
            }
            else if (embeddingConnection.ConnectionType == ConnectionTypeEnum.AzureAiSearch)
            {
                var aiSearchConnectionParts = embeddingConnection.ConnectionString.Split(';');
                if (aiSearchConnectionParts.Length != 3)
                {
                    throw new Exception("Invalid connection string for Azure AI Search");
                }
                var aiSearchConfig = new AzureAISearchConfig
                {
                    Endpoint = $"https://{aiSearchConnectionParts[0]}.search.windows.net",
                    APIKey = aiSearchConnectionParts[1],
                    UseHybridSearch = Convert.ToBoolean(aiSearchConnectionParts[2]),
                    Auth = AzureAISearchConfig.AuthTypes.APIKey
                };
                memoryBuilder = memoryBuilder.WithAzureAISearchMemoryDb(aiSearchConfig);
            }

            memoryBuilder = memoryBuilder.WithoutTextGenerator()
                .WithSimpleFileStorage(simpleFileStorageConfig)
                .WithContentDecoder<ExcelDataReaderDecoder>();
            return memoryBuilder.Build<MemoryServerless>();
        }
    }

    public interface IKernelMemoryProvider
    {
        MemoryServerless GetKernelMemory(EmbeddingConnectionModel embeddingConnection);
    }
}
