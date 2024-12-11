namespace AiCore.FileIngestion.Service.Common
{
    internal sealed class SelfDestructionMiddleware : IDisposable
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly Config _config;
        private readonly ILogger<SelfDestructionMiddleware> _logger;

        public SelfDestructionMiddleware(
            IHostApplicationLifetime applicationLifetime,
            Config config,
            ILogger<SelfDestructionMiddleware> logger)
        {
            _applicationLifetime = applicationLifetime;
            _config = config;
            _logger = logger;
        }

        private readonly CancellationTokenSource _cts = new();

        public async Task Invoke(HttpContext context, RequestDelegate next)
        {
            using var localCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            var requestTask = next(context);
            var softTimeoutTask = Task.Delay(TimeSpan.Parse(_config.IngestionTimeout), localCts.Token);
            if (await Task.WhenAny(requestTask, softTimeoutTask) == softTimeoutTask)
            {
                if (!softTimeoutTask.IsCanceled)
                {
                    _logger.LogError("Encountered a long-running request at '{Path}'. Shutting down.", context.Request.Path);
                    await _cts.CancelAsync();
                    // initiate graceful shutdown, give a chance for started requests to complete (the system actually waits for them)
                    _applicationLifetime.StopApplication();
                    await Task.WhenAny(requestTask, DieEventually());
                }
                // need to hang here while app is stopping, otherwise clients get 200 OK
                await requestTask;
            }
            else
            {
                await localCts.CancelAsync();
            }
        }

        private async Task DieEventually()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            _logger.LogInformation("DieEventually");
            Environment.Exit(1);
        }

        public void Dispose()
        {
            _cts.Dispose();
        }
    }
}
