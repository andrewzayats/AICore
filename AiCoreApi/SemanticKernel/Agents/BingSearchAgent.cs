using Microsoft.SemanticKernel;
using AiCoreApi.Models.DbModels;
using System.Web;
using AiCoreApi.Common;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Data.Processors;

namespace AiCoreApi.SemanticKernel.Agents
{
    public class BingSearchAgent : BaseAgent, IBingSearchAgent
    {
        private const string DebugMessageSenderName = "BingSearchAgent";
        private readonly Uri? _uri = new("https://api.bing.microsoft.com/v7.0/search?q");

        public static class AgentPromptPlaceholders
        {
            public const string HasFilesPlaceholder = "hasFiles";
            public const string FilesNamesPlaceholder = "filesNames";
            public const string FilesDataPlaceholder = "filesData";
        }

        private static class AgentContentParameters
        {
            public const string QueryString = "queryString";
            public const string BingConnection = "bingConnection";
            public const string Count = "count";
        }
        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnectionProcessor _connectionProcessor;

        public BingSearchAgent(
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor,
            IHttpClientFactory httpClientFactory,
            IConnectionProcessor connectionProcessor,
            ExtendedConfig extendedConfig,
            ILogger<BingSearchAgent> logger) : base(requestAccessor, extendedConfig, logger)
        {
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor;
            _httpClientFactory = httpClientFactory;
            _connectionProcessor = connectionProcessor;
        }

        public override async Task<string> DoCall(
            AgentModel agent, 
            Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(p => parameters[p.Key] = HttpUtility.HtmlDecode(p.Value));

            var queryString = ApplyParameters(agent.Content[AgentContentParameters.QueryString].Value, parameters);
            queryString = ApplyParameters(queryString, new Dictionary<string, string>
            {
                {AgentPromptPlaceholders.HasFilesPlaceholder, _requestAccessor.MessageDialog.Messages.Last().HasFiles().ToString()},
                {AgentPromptPlaceholders.FilesDataPlaceholder, _requestAccessor.MessageDialog.Messages.Last().GetFileContents()},
                {AgentPromptPlaceholders.FilesNamesPlaceholder, _requestAccessor.MessageDialog.Messages.Last().GetFileNames()}
            });
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Execute Query String", queryString);

            var bingConnectionName = agent.Content[AgentContentParameters.BingConnection].Value;
            var connections = await _connectionProcessor.List();
            var bingConnection = GetConnection(_requestAccessor, _responseAccessor, connections, ConnectionType.BingApi, DebugMessageSenderName, connectionName: bingConnectionName);

            var results = await DoSearchAsync(
                queryString,
                bingConnection.Content["bingApiKey"], 
                int.Parse(agent.Content[AgentContentParameters.Count].Value));
            var result = results.ToJson();

            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Execute Query String Result", result);
            return result;
        }

        private async Task<List<string>?> DoSearchAsync(string query, string apiKey, int count = 1, int offset = 0, CancellationToken cancellationToken = default)
        {
            if (count is <= 0 or >= 50)
                throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} value must be greater than 0 and less than 50.");

            var uri = new Uri($"{_uri}={Uri.EscapeDataString(query.Trim())}&count={count}&offset={offset}");
            using var response = await SendGetRequestAsync(uri, apiKey, cancellationToken).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var webPages = json.JsonGet<List<WebPage>>("webPages.value");
            return webPages?.Select(x => x.Snippet).Take(count).ToList();
        }

        private async Task<HttpResponseMessage> SendGetRequestAsync(Uri uri, string apiKey, CancellationToken cancellationToken = default)
        {
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            if (!string.IsNullOrEmpty(apiKey))
                httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
            else
                throw new InvalidOperationException("Bing API key is not set.");
            using var httpClient = _httpClientFactory.CreateClient("RetryClient");
            return await httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
        }

        public class WebPage
        {
            public string Name { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Snippet { get; set; } = string.Empty;
        }
    }

    public interface IBingSearchAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
