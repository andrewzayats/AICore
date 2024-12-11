using AiCoreApi.Models.DbModels;
namespace AiCoreApi.Services.IngestionServices
{
    internal class DataIngestionWorkerFactory : IDataIngestionWorkerFactory
    {
        private readonly ISharePointIngestionService _sharePointIngestionService;
        private readonly IFileUploadIngestionService _fileUploadIngestionService;
        private readonly IWebUrlIngestionService _webUrlIngestionService;


        public DataIngestionWorkerFactory(
            ISharePointIngestionService sharePointIngestionService,
            IFileUploadIngestionService fileUploadIngestionService,
            IWebUrlIngestionService webUrlIngestionService)
        {
            _sharePointIngestionService = sharePointIngestionService;
            _fileUploadIngestionService = fileUploadIngestionService;
            _webUrlIngestionService = webUrlIngestionService;
        }

        public IDataIngestionWorker GetService(IngestionModel ingestion) => GetService(ingestion.Type);
        public IDataIngestionWorker GetService(IngestionType ingestionType)
        {
            switch (ingestionType)
            {
                case IngestionType.SharePoint:
                    return _sharePointIngestionService;
                case IngestionType.WebUrl:
                    return _webUrlIngestionService;
                case IngestionType.UploadFile:
                    return _fileUploadIngestionService;
                default:
                    throw new InvalidOperationException($"Unsupported data source '{ingestionType}'.");
            }
        }
    }

    public interface IDataIngestionWorkerFactory
    {
        IDataIngestionWorker GetService(IngestionModel ingestion);
        IDataIngestionWorker GetService(IngestionType ingestionType);
    }

    public interface IDataIngestionWorker
    {
        Task Process(IngestionModel ingestion, int taskId);

        Task<List<string>> GetAutoComplete(string parameterName, IngestionModel ingestionModel) => Task.FromResult(new List<string>());
    }
}
