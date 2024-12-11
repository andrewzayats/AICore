using Microsoft.SemanticKernel;
using AiCoreApi.Models.DbModels;
using System.Net.Http.Headers;
using System.Text;
using AiCoreApi.Common;
using System.Web;

namespace AiCoreApi.SemanticKernel.Agents
{
    public class ApiCallAgent : BaseAgent, IApiCallAgent
    {
        private const string DebugMessageSenderName = "ApiCallAgent";
        private static class AgentContentParameters
        {
            public const string Url = "url";
            public const string HttpMethod = "httpMethod";
            public const string ContentType = "contentType";
            public const string Body = "body";
            public const string Authentication = "authentication";
            public const string CustomHeaderName = "customHeaderName";
            public const string CustomHeaderValue = "customHeaderValue";
        }

        private readonly ILogger<ApiCallAgent> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ResponseAccessor _responseAccessor;
        private readonly RequestAccessor _requestAccessor;

        public ApiCallAgent(
            ILogger<ApiCallAgent> logger,
            IHttpClientFactory httpClientFactory, 
            ResponseAccessor responseAccessor,
            RequestAccessor requestAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _responseAccessor = responseAccessor;
            _requestAccessor = requestAccessor;
        }

        public override async Task<string> DoCall(AgentModel agent, Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(p => parameters[p.Key] = HttpUtility.HtmlDecode(p.Value));

            var url = ApplyParameters(agent.Content[AgentContentParameters.Url].Value, parameters);
            var uri = new Uri(url);
            using var httpRequestMessage = new HttpRequestMessage(GetHttpMethod(agent), uri);
            var body = string.Empty;
            if (agent.Content.ContainsKey(AgentContentParameters.Body) 
                && !string.IsNullOrWhiteSpace(agent.Content[AgentContentParameters.Body].Value))
            {
                body = ApplyParameters(agent.Content[AgentContentParameters.Body].Value, parameters);
                if (agent.Content.ContainsKey(AgentContentParameters.ContentType) 
                    && !string.IsNullOrWhiteSpace(agent.Content[AgentContentParameters.ContentType].Value))
                {
                    var contentType = agent.Content[AgentContentParameters.ContentType].Value;
                    httpRequestMessage.Content = new StringContent(body, Encoding.UTF8, contentType);
                }
            }
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "DoCall Request", $"{GetHttpMethod(agent)}: {uri}\r\nBody: \r\n{body}");
            using var httpClient = GetHttpClient(agent, parameters);
            using var response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "DoCall Response", responseBody);

            _logger.LogInformation("{Login}, Action:{Action}, Agent: {Agent}, Url: {url}",
                _requestAccessor.Login, "ApiCall", agent.Name, url);
            return responseBody;
        }

        private HttpClient GetHttpClient(AgentModel agent, Dictionary<string, string> parameters)
        {
            var httpClient = _httpClientFactory.CreateClient("RetryClient");
            if (agent.Content.ContainsKey(AgentContentParameters.Authentication) 
                && !string.IsNullOrWhiteSpace(agent.Content[AgentContentParameters.Authentication].Value))
            {
                var authentication = agent.Content[AgentContentParameters.Authentication].Value;
                var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(authentication));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
                return httpClient;
            }
            if (agent.Content.ContainsKey(AgentContentParameters.CustomHeaderName) && agent.Content.ContainsKey(AgentContentParameters.CustomHeaderValue))
            {
                var customHeaderName = ApplyParameters(agent.Content[AgentContentParameters.CustomHeaderName].Value, parameters);
                var customHeaderValue = ApplyParameters(agent.Content[AgentContentParameters.CustomHeaderValue].Value, parameters);
                if (!string.IsNullOrWhiteSpace(customHeaderName) && !string.IsNullOrWhiteSpace(customHeaderValue))
                   httpClient.DefaultRequestHeaders.Add(customHeaderName, customHeaderValue);
            }
            return httpClient;
        }

        private HttpMethod GetHttpMethod(AgentModel agent)
        {
            if(!agent.Content.ContainsKey(AgentContentParameters.HttpMethod))
                return HttpMethod.Get;

            var httpMethod = agent.Content[AgentContentParameters.HttpMethod].Value;
            return httpMethod switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "DELETE" => HttpMethod.Delete,
                _ => HttpMethod.Get
            };
        }
    }

    public interface IApiCallAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
