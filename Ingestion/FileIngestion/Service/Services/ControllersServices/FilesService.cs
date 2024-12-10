using AiCore.FileIngestion.Service.Common;
using AiCore.FileIngestion.Service.Common.Handlers;
using AiCore.FileIngestion.Service.Models.ViewModels;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Handlers;

namespace AiCore.FileIngestion.Service.Services.ControllersServices
{
    public class FilesService : IFilesService
    {
        private readonly IKernelMemoryProvider _kernelMemoryProvider;
        private readonly IServiceProvider _serviceProvider;
        public FilesService(
            IKernelMemoryProvider kernelMemoryProvider, 
            IServiceProvider serviceProvider)
        {
            _kernelMemoryProvider = kernelMemoryProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task Add(UploadFileRequestModel request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                throw new RequestValidationException("Invalid document ID, it must not be NULL or empty");
            }

            // WARNING: tags starting with the reserved prefix "__" will not throw, though they should
            // Uncomment the respective condition below when AI Core tags do not start with "__".
            var invalidTags = string.Join(", ", request.Tags.Keys
                .Where(key => /*key.StartsWith(Constants.ReservedTagsPrefix) ||*/ key is Constants.ReservedDocumentIdTag
                    or Constants.ReservedFileIdTag
                    or Constants.ReservedFilePartitionTag
                    or Constants.ReservedFileTypeTag
                    or Constants.ReservedSyntheticTypeTag)
                .Select(key => $"'{key}'"));
            if (!string.IsNullOrEmpty(invalidTags))
            {
                throw new RequestValidationException($"Invalid document tags, the tag names {invalidTags} are reserved for internal use.");
            }

            var tagsCollection = new TagCollection();
            foreach (var requestTag in request.Tags)
            {
                if (string.IsNullOrWhiteSpace(requestTag.Key)) continue;
                var values = requestTag.Value.Select(value => string.IsNullOrWhiteSpace(value) ? null : value).ToList();
                tagsCollection.Add(requestTag.Key, values);
            }

            var document = new Document(request.Id, tagsCollection);
            using var memoryStream = new MemoryStream(request.Content);
            document.AddStream(request.Name, memoryStream);
            var kernelMemory = _kernelMemoryProvider.GetKernelMemory(request.EmbeddingConnection);

            kernelMemory.Orchestrator.AddHandler<TranslateStepHandler>("translate_text");
            kernelMemory.Orchestrator.AddHandler<TextExtractionHandler>("extract_text");
            kernelMemory.Orchestrator.AddHandler<TextPartitioningHandler>("split_text_in_partitions");
            kernelMemory.Orchestrator.AddHandler<GenerateEmbeddingsHandler>("generate_embeddings");
            kernelMemory.Orchestrator.AddHandler<SaveRecordsHandler>("save_memory_records");

            var steps = new List<string> { "extract_text" };
            if(request.TranslateStep.Enabled)
            {
                steps.Add("translate_text");
            }
            steps.Add("split_text_in_partitions");
            steps.Add("generate_embeddings");
            steps.Add("save_memory_records");

            RequestContextAccessor.UploadFileRequestModel.Value = request;
            await kernelMemory.ImportDocumentAsync(document, index: request.EmbeddingConnection.IndexName,
                cancellationToken: cancellationToken, steps: steps);

        }

        public async Task Delete(EmbeddingConnectionModel embeddingConnectionModel, string id, CancellationToken cancellationToken = default)
        {
            var kernelMemory = _kernelMemoryProvider.GetKernelMemory(embeddingConnectionModel);
            await kernelMemory.DeleteDocumentAsync(id, index: embeddingConnectionModel.IndexName, cancellationToken: cancellationToken);
        }
    }

    public class RequestValidationException : Exception
    {
        public RequestValidationException(string? message) : base(message) { }
    }

    public interface IFilesService
    {
        Task Add(UploadFileRequestModel request, CancellationToken cancellationToken = default);
        Task Delete(EmbeddingConnectionModel embeddingConnectionModel, string id, CancellationToken cancellationToken = default);
    }
}