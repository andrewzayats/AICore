namespace AiCoreApi.Common
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AiCoreUiException ex)
            {
                _logger.LogError(ex, "AiCoreUiException exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, AiCoreUiException exception)
        {
            var response = new { message = exception.Message };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return context.Response.WriteAsJsonAsync(response);
        }

        public class AiCoreUiException: Exception
        {
            public AiCoreUiException(string message) : base(message)
            {
            }
        }
    }
}
