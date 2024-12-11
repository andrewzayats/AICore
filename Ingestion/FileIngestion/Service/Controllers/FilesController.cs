using AiCore.FileIngestion.Service.Models.ViewModels;
using AiCore.FileIngestion.Service.Services.ControllersServices;
using Microsoft.AspNetCore.Mvc;

namespace AiCore.FileIngestion.Service.Controllers
{
    [Route("files")]
    [ApiController]
    public class FileIngestionController : ControllerBase
    {
        private readonly IFilesService _filesService;
        private readonly ILogger<FileIngestionController> _logger;

        public FileIngestionController(
            IFilesService filesService,
            ILogger<FileIngestionController> logger)
        {
            _filesService = filesService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromBody] UploadFileRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogTrace("New upload HTTP request, content length {Length}", request.Content.Length);
                await _filesService.Add(request, cancellationToken);
                return Ok();
            }
            catch (RequestValidationException e)
            {
                return ValidationProblem(new ValidationProblemDetails
                {
                    Title = "Validation error",
                    Detail = e.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Errors = { { "Validation error", new[] { e.Message } } }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Document upload failed");
                return Problem(title: "Document upload failed", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile([FromRoute] string fileId, [FromBody] EmbeddingConnectionModel embeddingConnectionModel, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogTrace("New delete document HTTP request, id '{docId}'", fileId);
                await _filesService.Delete(embeddingConnectionModel, fileId, cancellationToken);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Document delete failed");
                return Problem(title: "Document delete failed", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
