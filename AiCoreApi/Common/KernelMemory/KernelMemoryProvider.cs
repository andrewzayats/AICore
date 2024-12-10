using AiCoreApi.Models.DbModels;
using Microsoft.KernelMemory;

namespace AiCoreApi.Common.KernelMemory
{
    public sealed class KernelMemoryProvider : IKernelMemoryProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExtendedConfig _config;

        public KernelMemoryProvider(
            IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory,
            ExtendedConfig config)
            
        {
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public IKernelMemory GetKernelMemory(ConnectionModel llmConnection, ConnectionModel embeddingConnection, ConnectionModel? vectorDbConnection)
        {
            var httpContext = _serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
            var serviceProvider = httpContext?.RequestServices ?? _serviceProvider;

            var qdConfig = new QdrantConfig { Endpoint = _config.QdrantUrl };
            var httpClient = _httpClientFactory.CreateClient("RetryClient");
            var azureOpenAiEmbeddingConfig = new AzureOpenAIConfig
            {
                Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                APIKey = embeddingConnection.Content["apiKey"],
                Endpoint = embeddingConnection.Content["endpoint"],
                Deployment = embeddingConnection.Content["modelName"],
                MaxTokenTotal = Convert.ToInt32(embeddingConnection.Content["maxTokens"])
            };
            var searchClientConfig = new SearchClientConfig
            {
                EmptyAnswer = _config.NoInformationFoundText,
                AnswerTokens = Convert.ToInt32(llmConnection.Content["maxAnswersTokens"]),
                Temperature = Convert.ToDouble(llmConnection.Content["temperature"]),
            };
            var azureOpenAiTextConfig = new AzureOpenAIConfig
            {
                Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                APIKey = llmConnection.Content["azureOpenAiKey"],
                Endpoint = llmConnection.Content["endpoint"],
                Deployment = llmConnection.Content["deploymentName"],
                MaxTokenTotal = Convert.ToInt32(llmConnection.Content["maxRequestTokens"]),
                MaxRetries = 5,
                APIType = AzureOpenAIConfig.APITypes.ChatCompletion
            };
            var memoryBuilder = new KernelMemoryBuilder()
                .WithCustomPromptProvider(new MarkdownPromptProvider(serviceProvider))
                .WithSearchClientConfig(searchClientConfig)
                .WithAzureOpenAITextGeneration(azureOpenAiTextConfig, httpClient: httpClient)
                .WithAzureOpenAITextEmbeddingGeneration(azureOpenAiEmbeddingConfig);
            if (vectorDbConnection != null)
            {
                var aiSearchConfig = new AzureAISearchConfig
                {
                    Endpoint = $"https://{vectorDbConnection.Content["resourceName"]}.search.windows.net",
                    APIKey = vectorDbConnection.Content["apiKey"],
                    UseHybridSearch = vectorDbConnection.Content.ContainsKey("useHybridSearch") && vectorDbConnection.Content["useHybridSearch"].ToLower() == "true",
                    Auth = AzureAISearchConfig.AuthTypes.APIKey
                };
                memoryBuilder = memoryBuilder.WithAzureAISearchMemoryDb(aiSearchConfig);
            }
            else
            {
                memoryBuilder = memoryBuilder.WithQdrantMemoryDb(qdConfig);
            }
            return memoryBuilder.Build<MemoryServerless>();
        }

    }

    public interface IKernelMemoryProvider
    {
        IKernelMemory GetKernelMemory(ConnectionModel llmConnection, ConnectionModel embeddingConnection, ConnectionModel? vectorDbConnection);
    }
}
