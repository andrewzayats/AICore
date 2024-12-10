using AiCoreApi.Models.ViewModels;
using AiCoreApi.Common;
using AiCoreApi.SemanticKernel;
using AiCoreApi.SemanticKernel.Agents;
using AiCoreApi.Common.Extensions;

namespace AiCoreApi.Services.ControllersServices
{
    public class CopilotService : ICopilotService
    {
        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;
        private readonly IPlanner _planner;
        private readonly IWhisperAgent _whisperAgent;
        private readonly IPromptAgent _promptAgent;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IVectorSearchAgent _vectorSearchAgent;
        private readonly ILogger _logger;

        public CopilotService(
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor,
            IPlanner planner,
            IWhisperAgent whisperAgent,
            IPromptAgent promptAgent,
            IHttpClientFactory httpClientFactory,
            IVectorSearchAgent vectorSearchAgent,
            ILogger<CopilotService> logger)
        {
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor;
            _planner = planner;
            _whisperAgent = whisperAgent;
            _promptAgent = promptAgent;
            _httpClientFactory = httpClientFactory;
            _vectorSearchAgent = vectorSearchAgent;
            _logger = logger;
        }

        public async Task<string> Prompt(string prompt, double temperature, string connectionName)
        {
            var response = await _promptAgent.Prompt(prompt, temperature, connectionName);
            return response;
        }

        public async Task<MessageDialogViewModel> Chat()
        {
            var messageDialog = _requestAccessor.MessageDialog!;
            try
            {
                var response = await _planner.GetChatResponse();
                messageDialog.Messages!.Add(response);
            }
            // Tokens limit reached
            catch (TokensLimitException ex)
            {
                messageDialog.Messages!.Add(new MessageDialogViewModel.Message
                {
                    Sender = PlannerHelpers.AssistantName, 
                    Text = ex.Message,
                    DebugMessages = _responseAccessor.CurrentMessage.DebugMessages
                });
            }
            messageDialog.ClearFilesContent();
            _logger.LogDebug("Chat response generated for: {Login}, Tokens spent: {Spent}, Request: {Request}, Response: {Response}", 
                _requestAccessor.Login, 
                _responseAccessor.CurrentMessage.SpentTokens.ToJson(), 
                _requestAccessor.MessageDialog.Messages.Last().Text, 
                _responseAccessor.CurrentMessage.Text);
            return messageDialog;
        }

        public async Task<List<SearchItemModel>?> Search()
        {
            var vectorSearchResult = await _vectorSearchAgent.Search(_requestAccessor.Query, -1, 0, "", "", "", null);
            return vectorSearchResult.JsonGet<List<SearchItemModel>>();
        }

        public async Task<string> Transcribe(IFormFile file)
        {
            if (file.Length == 0)
                return string.Empty;
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }
            return await _whisperAgent.Transcribe(Convert.ToBase64String(fileBytes), "webm", "audio/webm");
        }

        public async Task<string> Proxy(ProxyRequestModel proxyRequest)
        {
            using var httpClient = _httpClientFactory.CreateClient("RetryClient");
            var method = proxyRequest.Method switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "DELETE" => HttpMethod.Delete,
                _ => HttpMethod.Get
            };
            foreach(var header in proxyRequest.Headers)
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            using var httpRequestMessage = new HttpRequestMessage(method, new Uri(proxyRequest.Url));
            if (!string.IsNullOrEmpty(proxyRequest.Body)){ 
                httpRequestMessage.Content = new StringContent(proxyRequest.Body, System.Text.Encoding.UTF8, proxyRequest.ContentType);
            }
            using var response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    public interface ICopilotService
    {
        Task<string> Prompt(string prompt, double temperature, string connectionName);
        Task<MessageDialogViewModel> Chat();
        Task<List<SearchItemModel>?> Search();
        Task<string> Transcribe(IFormFile file);
        Task<string> Proxy(ProxyRequestModel proxyRequest);
    }
}
