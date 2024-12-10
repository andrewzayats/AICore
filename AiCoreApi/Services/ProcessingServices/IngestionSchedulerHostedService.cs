using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;

namespace AiCoreApi.Services.ProcessingServices
{
    internal class IngestionSchedulerHostedService : IHostedService, IDisposable
    {
        private const string ServiceName = "Scheduler";
        private const int CheckIntervalSeconds = 600;
        private const int MaxTasksCount = 2;

        private readonly IIngestionProcessor _ingestionProcessor;
        private readonly ITaskProcessor _taskProcessor;
        private readonly ILogger<IngestionSchedulerHostedService> _logger;

        private Timer? _timer;

        public IngestionSchedulerHostedService(
            IIngestionProcessor ingestionProcessor,
            ITaskProcessor taskProcessor,
            ILogger<IngestionSchedulerHostedService> logger)
        {
            _ingestionProcessor = ingestionProcessor;
            _taskProcessor = taskProcessor;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _taskProcessor.ResetUnfinishedTasks();

            _timer = new Timer(Process, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        public async void Process(object? state)
        {
            try
            {
                await ProcessSync();

                await _taskProcessor.ClearHistory();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{ServiceName} service error.");
            }
            finally
            {
                _timer?.Change(TimeSpan.FromSeconds(CheckIntervalSeconds), Timeout.InfiniteTimeSpan);
            }
        }

        private async Task ProcessSync()
        {
            var ingestions = _ingestionProcessor.GetStale().Take(MaxTasksCount);
            foreach (var ingestion in ingestions)
            {
                var tasks = _taskProcessor.GetByIngestion(ingestion.IngestionId);
                var active = tasks.FirstOrDefault(t =>
                    t.Type == TaskType.DataSync && t.State is TaskState.InProgress or TaskState.New);
                if (active != null)
                {
                    return;
                }

                var task = new TaskModel
                {
                    IngestionId = ingestion.IngestionId,
                    Type = TaskType.DataSync,
                    CreatedBy = ServiceName,
                };
                await _taskProcessor.ScheduleTask(task);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
