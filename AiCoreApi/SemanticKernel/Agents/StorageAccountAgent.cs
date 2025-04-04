using System.Web;
using Microsoft.SemanticKernel;
using AiCoreApi.Models.DbModels;
using AiCoreApi.Common;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Data.Processors;
using Azure.Storage.Blobs;
using Azure.Storage;

namespace AiCoreApi.SemanticKernel.Agents
{
    public class StorageAccountAgent : BaseAgent, IStorageAccountAgent
    {
        private const string DebugMessageSenderName = "StorageAccountAgent";
        public static class AgentPromptPlaceholders
        {
            public const string FileDataPlaceholder = "firstFileData";
        }
        private static class AgentContentParameters
        {
            public const string Base64Content = "base64Content";
            public const string FileName = "fileName";
            public const string Action = "action";
            public const string ConnectionName = "connectionName";
            public const string ContainerName = "containerName";
        }

        private readonly IEntraTokenProvider _entraTokenProvider;
        private readonly IConnectionProcessor _connectionProcessor;
        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;

        public StorageAccountAgent(
            IEntraTokenProvider entraTokenProvider,
            IConnectionProcessor connectionProcessor,
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor,
            ExtendedConfig extendedConfig,
            ILogger<StorageAccountAgent> logger) : base(requestAccessor, extendedConfig, logger)
        {
            _entraTokenProvider = entraTokenProvider;
            _connectionProcessor = connectionProcessor;
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor;
        }

        public override async Task<string> DoCall(
            AgentModel agent,
            Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(p => parameters[p.Key] = HttpUtility.HtmlDecode(p.Value));

            var action = agent.Content[AgentContentParameters.Action].Value;
            var connectionName = agent.Content[AgentContentParameters.ConnectionName].Value;
            var containerName = ApplyParameters(agent.Content.ContainsKey(AgentContentParameters.ContainerName) ? agent.Content[AgentContentParameters.ContainerName].Value : string.Empty, parameters);
            var fileName = ApplyParameters(agent.Content.ContainsKey(AgentContentParameters.FileName) ? agent.Content[AgentContentParameters.FileName].Value : string.Empty, parameters);
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "DoCall Request", $"Action: {action}\r\nConnection: {connectionName}\r\nContainerName: {containerName}\r\nFileName: {fileName}");

            var connections = await _connectionProcessor.List();
            var connection = GetConnection(_requestAccessor, _responseAccessor, connections, ConnectionType.StorageAccount, DebugMessageSenderName, connectionName: connectionName);

            var accountName = connection.Content["accountName"];
            var accessType = connection.Content.ContainsKey("accessType") ? connection.Content["accessType"] : "apiKey";

            BlobServiceClient blobServiceClient;
            var blobEndpoint = $"https://{accountName}.blob.core.windows.net";
            if (accessType == "apiKey")
            {
                var accountKey = connection.Content["apiKey"];
                var storageCredentials = new StorageSharedKeyCredential(accountName, accountKey);
                blobServiceClient = new BlobServiceClient(new Uri(blobEndpoint), storageCredentials);
            }
            else
            {
                var accessToken = await _entraTokenProvider.GetAccessTokenObjectAsync(accessType, "https://storage.azure.com/.default");
                blobServiceClient = new BlobServiceClient(new Uri(blobEndpoint), new StaticTokenCredential(accessToken.Token, accessToken.ExpiresOn));
            }

            string result;
            switch (action)
            {
                case ("LIST"):
                    {
                        result = await ListContent(blobServiceClient, containerName, fileName);
                        break;
                    }
                case ("CONTAINERS"):
                    {
                        result = await ListContainers(blobServiceClient);
                        break;
                    }
                case ("ADD"):
                    {
                        var base64Content = ApplyParameters(agent.Content[AgentContentParameters.Base64Content].Value, parameters);
                        if (_requestAccessor.MessageDialog != null && _requestAccessor.MessageDialog.Messages!.Last().HasFiles())
                        {
                            base64Content = ApplyParameters(base64Content, new Dictionary<string, string> {
                            { AgentPromptPlaceholders.FileDataPlaceholder, _requestAccessor.MessageDialog.Messages!.Last().Files!.First().Base64Data } });
                        }
                        result = await Add(blobServiceClient, containerName, fileName, base64Content.StripBase64());
                        break;
                    }
                case ("DELETE"):
                    {
                        result = await Delete(blobServiceClient, containerName, fileName);
                        break;
                    }
                case ("GET"):
                    {
                        result = await Get(blobServiceClient, containerName, fileName);
                        break;
                    }
                default: throw new InvalidDataException($"Wrong action: {action}");
            }
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "DoCall Response", result);
            return result;
        }

        private async Task<string> ListContainers(BlobServiceClient blobServiceClient)
        {
            var containers = new List<object>();
            await foreach (var containerItem in blobServiceClient.GetBlobContainersAsync())
            {
                containers.Add(new { Name = containerItem.Name });
            }
            return containers.ToJson();
        }

        private async Task<string> ListContent(BlobServiceClient blobServiceClient, string containerName, string prefix = null)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = containerClient.GetBlobs(prefix: prefix);
            var result = blobs.Select(selector: blob => new BlobFile
            {
                Name = blob.Name,
                Size = blob.Properties.ContentLength
            }).ToList();

            return result.ToJson();
        }

        private async Task<string> Add(BlobServiceClient blobServiceClient, string containerName, string fileName, string base64Content)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var bytes = Convert.FromBase64String(base64Content);
            using (var stream = new MemoryStream(bytes))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
            return string.Empty;
        }

        private async Task<string> Delete(BlobServiceClient blobServiceClient, string containerName, string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobIfExistsAsync(fileName);
            return string.Empty;
        }

        private async Task<string> Get(BlobServiceClient blobServiceClient, string containerName, string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var blobDownloadInfo = await blobClient.DownloadAsync();
            using (var memoryStream = new MemoryStream())
            {
                await blobDownloadInfo.Value.Content.CopyToAsync(memoryStream);
                var result = memoryStream.ToArray();
                return Convert.ToBase64String(result);
            }
        }

        public class BlobFile
        {
            public string Name { get; set; }
            public long? Size { get; set; }
        }
    }

    public interface IStorageAccountAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
