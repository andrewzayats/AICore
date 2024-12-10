using AiCoreApi.Common;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using Microsoft.SemanticKernel;

namespace AiCoreApi.SemanticKernel
{
    public class SemanticKernelProvider: ISemanticKernelProvider
    {
        private readonly RequestAccessor _requestAccessor;
        private readonly IConnectionProcessor _connectionProcessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public SemanticKernelProvider(
            RequestAccessor requestAccessor,
            IConnectionProcessor connectionProcessor,
            IHttpClientFactory httpClientFactory)
        {
            _requestAccessor = requestAccessor;
            _connectionProcessor = connectionProcessor;
            _httpClientFactory = httpClientFactory;
        }

        public Kernel GetKernel(ConnectionModel connectionModel)
        {
            var httpClient = _httpClientFactory.CreateClient("RetryClient");
            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    connectionModel.Content["deploymentName"],
                    connectionModel.Content["endpoint"],
                    connectionModel.Content["azureOpenAiKey"],
                    httpClient: httpClient)
                .Build();
            return kernel;
        }

        public async Task<Kernel> GetKernel()
        {
            // Get the default LLM connection
            var connections = await _connectionProcessor.List();
            var llmConnection = connections.FirstOrDefault(conn =>
                conn.Type == ConnectionType.AzureOpenAiLlm &&
                _requestAccessor.DefaultConnectionNames.Contains(conn.Name)) 
                    ?? connections.FirstOrDefault(conn => conn.Type == ConnectionType.AzureOpenAiLlm);
            if (llmConnection == null)
                throw new Exception("No any LLM connection found");

            return GetKernel(llmConnection);
            
        }
    }

    public interface ISemanticKernelProvider
    {
        Kernel GetKernel(ConnectionModel connectionModel);
        Task<Kernel> GetKernel();
    }
}
