using AiCore.FileIngestion.Service.Models.ViewModels;

namespace AiCore.FileIngestion.Service.Common
{
    public class RequestContextAccessor
    {
        public static AsyncLocal<UploadFileRequestModel> UploadFileRequestModel = new();
    }
}
