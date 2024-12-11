using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Pipeline;

namespace AiCore.FileIngestion.Service.Common.Handlers
{
    public sealed class TranslateStepHandler : IPipelineStepHandler
    {
        private readonly IPipelineOrchestrator _orchestrator;
        private readonly ILogger<TranslateStepHandler> _log;
        const string Endpoint = "https://api.cognitive.microsofttranslator.com";

        public string StepName { get; }

        public TranslateStepHandler(
            string stepName,
            IPipelineOrchestrator orchestrator,
            ILogger<TranslateStepHandler> log)
        {
            _log = log;
            _orchestrator = orchestrator;
            StepName = stepName;
        }

        public async Task<(ReturnType returnType, DataPipeline updatedPipeline)> InvokeAsync(DataPipeline pipeline, CancellationToken cancellationToken = default)
        {
            foreach (var uploadedFile in pipeline.Files)
            {
                var summaryFiles = new Dictionary<string, DataPipeline.GeneratedFileDetails>();
                foreach (var generatedFile in uploadedFile.GeneratedFiles)
                {
                    var file = generatedFile.Value;
                    if (file.AlreadyProcessedBy(this))
                        continue;

                    // Translate only the original content
                    if (file.ArtifactType != DataPipeline.ArtifactTypes.ExtractedText)
                        continue;

                    switch (file.MimeType)
                    {
                        case MimeTypes.PlainText:
                        case MimeTypes.MarkDown:
                            var content = (await this._orchestrator.ReadFileAsync(pipeline, file.Name, cancellationToken).ConfigureAwait(false)).ToString();
                            (string summary, bool success) = await TranslateAsync(content).ConfigureAwait(false);
                            if (success)
                            {
                                var summaryData = new BinaryData(summary);
                                var destFile = uploadedFile.GetHandlerOutputFileName(this);
                                await this._orchestrator.WriteFileAsync(pipeline, destFile, summaryData, cancellationToken).ConfigureAwait(false);

                                summaryFiles.Add(destFile, new DataPipeline.GeneratedFileDetails
                                {
                                    Id = Guid.NewGuid().ToString("N"),
                                    ParentId = uploadedFile.Id,
                                    Name = destFile,
                                    Size = summary.Length,
                                    MimeType = MimeTypes.PlainText,
                                    ArtifactType = DataPipeline.ArtifactTypes.SyntheticData,
                                    Tags = pipeline.Tags.Clone(),
                                    ContentSHA256 = CalculateSha256(summaryData),
                                });
                            }
                            break;
                        default:
                            continue;
                    }
                    file.MarkProcessedBy(this);
                }

                // Add new files to pipeline status
                foreach (var file in summaryFiles)
                {
                    file.Value.MarkProcessedBy(this);
                    uploadedFile.GeneratedFiles.Add(file.Key, file.Value);
                }
            }
            return (ReturnType.Success, pipeline);
        }
        private static string CalculateSha256(BinaryData binaryData) => Convert.ToHexString(SHA256.HashData(binaryData.ToMemory().Span)).ToLowerInvariant();

        private async Task<string> TranslateCall(string text, string toLanguage, string apiKey, string region)
        {
            try
            {
                using var httpClient = new HttpClient();
                var route = $"/translate?api-version=3.0&to={toLanguage}";
                var uri = new Uri(Endpoint + route);
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);
                var requestBody = new List<object> { new { Text = text } };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(jsonResponse);
                var detectedLanguage = document.RootElement[0].GetProperty("detectedLanguage").GetProperty("language").GetString();
                if(detectedLanguage == toLanguage)
                    return "";

                var translation = document.RootElement[0].GetProperty("translations")[0].GetProperty("text").GetString();
                return translation ?? "";
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error translating text");
                return "";
            }
        }

        private async Task<(string summary, bool success)> TranslateAsync(string content)
        {
            if (RequestContextAccessor.UploadFileRequestModel.Value == null || !RequestContextAccessor.UploadFileRequestModel.Value.TranslateStep.Enabled)
                return ("", false);

            var toLanguage = RequestContextAccessor.UploadFileRequestModel.Value.TranslateStep.TargetLanguage;
            var apiKey = RequestContextAccessor.UploadFileRequestModel.Value.TranslateStep.ApiKey;
            var region = RequestContextAccessor.UploadFileRequestModel.Value.TranslateStep.Region;

            if (string.IsNullOrWhiteSpace(toLanguage) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(region))
            {
                _log.LogWarning("No target language or API key or region specified, skipping translation");
                return ("", false);
            }
            var result = await TranslateCall(content, toLanguage, apiKey, region);
            return (result, !string.IsNullOrEmpty(result));
        }
    }
}