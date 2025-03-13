using AiCoreApi.Common;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using Microsoft.SemanticKernel;
using System.Web;
using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.Core.Pipeline;
using Newtonsoft.Json;

namespace AiCoreApi.SemanticKernel.Agents
{
    public class OcrClassifyDocumentAgent : BaseAgent, IOcrClassifyDocumentAgent
    {
        private const string DebugMessageSenderName = "OcrClassifyDocumentAgent";

        public static class AgentPromptPlaceholders
        {
            public const string FileDataPlaceholder = "firstFileData";
        }

        private static class AgentContentParameters
        {
            public const string DocumentIntelligenceConnection = "documentIntelligenceConnection";
            public const string Base64Image = "base64Image";
            public const string ClassifierId = "classifierID";
            public const string Pages = "pages";
            public const string SplitMode = "splitMode";
            public const string StringIndexType = "stringIndexType";
        }

        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnectionProcessor _connectionProcessor;
        private readonly ILogger<OcrClassifyDocumentAgent> _logger;

        public OcrClassifyDocumentAgent(
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor,
            IHttpClientFactory httpClientFactory,
            IConnectionProcessor connectionProcessor,
            ILogger<OcrClassifyDocumentAgent> logger)
        {
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor;
            _httpClientFactory = httpClientFactory;
            _connectionProcessor = connectionProcessor;
            _logger = logger;
        }

        public override async Task<string> DoCall(
            AgentModel agent,
            Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(p => parameters[p.Key] = HttpUtility.HtmlDecode(p.Value));

            var base64Image = ApplyParameters(agent.Content[AgentContentParameters.Base64Image].Value, parameters);
            if (_requestAccessor.MessageDialog != null && _requestAccessor.MessageDialog.Messages!.Last().HasFiles() && base64Image.Contains(AgentPromptPlaceholders.FileDataPlaceholder))
            {
                base64Image = ApplyParameters(base64Image, new Dictionary<string, string>
                {
                    {AgentPromptPlaceholders.FileDataPlaceholder, _requestAccessor.MessageDialog.Messages!.Last().Files!.First().Base64Data},
                });
            }

            var documentIntelligenceConnection = agent.Content[AgentContentParameters.DocumentIntelligenceConnection].Value;
            var connections = await _connectionProcessor.List();
            var connection = GetConnection(_requestAccessor, _responseAccessor, connections, ConnectionType.DocumentIntelligence, DebugMessageSenderName, connectionName: documentIntelligenceConnection);

            var stringIndexType = GetParameterValueOrNull(agent, AgentContentParameters.StringIndexType);
            var splitMode = GetParameterValueOrNull(agent, AgentContentParameters.SplitMode);
            var pages = ApplyParameters(GetParameterValueOrNull(agent, AgentContentParameters.Pages), parameters);

            var classifierId = ApplyParameters(agent.Content[AgentContentParameters.ClassifierId].Value, parameters);

            if (string.IsNullOrWhiteSpace(classifierId))
            {
                throw new ArgumentException($"{AgentContentParameters.ClassifierId} cannot be empty");
            }

            var endpoint = connection.Content["endpoint"];
            var apiKey = connection.Content["apiKey"];

            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "OCR Classifying", $"{endpoint}, {classifierId}, {apiKey} \r\nStringIndexType:{stringIndexType} SplitMode:{splitMode} Pages:{pages}");

            var result = await ProcessFileAsync(
                endpoint, 
                classifierId, 
                apiKey,
                base64Image.StripBase64(),
                stringIndexType,
                splitMode,
                pages
                );

            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "OCR Classifying Result", result);

            _logger.LogInformation("{Login}, Action:{Action}, ConnectionName: {ConnectionName}, ClassifierID: {ClassifierID}, StringIndexType: {StringIndexType}, SplitMode: {SplitMode}, Pages: {Pages}",
                _requestAccessor.Login, "Ocr Classifying", connection.Name, classifierId, stringIndexType, splitMode, pages);
            return result;
        }

        private static string? GetParameterValueOrNull(AgentModel agent, string optionName)
        {
            return !agent.Content.ContainsKey(optionName)
                ? null
                : string.IsNullOrWhiteSpace(agent.Content[optionName].Value)
                    ? null
                    : agent.Content[optionName].Value;
        }

        private async Task<string?> ProcessFileAsync(
            string modelUrl, 
            string classifierId, 
            string apiKey, 
            string base64Data, 
            string? stringIndexType, 
            string? splitMode,
            string? pages)
        {
            var file = Convert.FromBase64String(base64Data);
            _httpClientFactory.CreateClient("");
            var clientOptions = new DocumentIntelligenceClientOptions
            {
                Transport = new HttpClientTransport(_httpClientFactory.CreateClient("RetryClient"))
            };
            var client = new DocumentIntelligenceClient(new Uri(modelUrl), new AzureKeyCredential(apiKey), clientOptions);
            var content = new ClassifyDocumentContent
            {
                Base64Source = BinaryData.FromBytes(file),
            };

            var operation = await client.ClassifyDocumentAsync(WaitUntil.Completed, classifierId, content, stringIndexType ?? (StringIndexType?) null, splitMode ?? (SplitMode?) null,
                pages);

            var result = HandleResult(operation.Value);
            return result.ToJson();
        }

        private static OcrClassifyResult HandleResult(AnalyzeResult result)
        {
            var documents = result.Documents
                .Select(doc => new OcrClassifyDocument { DocType = doc.DocType, Confidence = doc.Confidence })
                .ToList();

            return new OcrClassifyResult
            {
                Documents = documents
            };
        }
    }

    public class OcrClassifyResult
    {
        public List<OcrClassifyDocument>? Documents { get; set; } = new();
    }

    public class OcrClassifyDocument
    {
        public string DocType { get; set; }

        public float Confidence { get; set; }
    }

    public interface IOcrClassifyDocumentAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
