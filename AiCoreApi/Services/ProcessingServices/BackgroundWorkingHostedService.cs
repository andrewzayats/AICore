using System.Runtime;
using AiCoreApi.Common;
using AiCoreApi.Services.ProcessingServices.AgentsHandlers;

namespace AiCoreApi.Services.ProcessingServices
{
    public class BackgroundWorkingHostedService : IHostedService
    {
        private readonly ISchedulerAgentService _schedulerAgentService;
        private readonly IBackgroundWorkerAgentService _backgroundWorkerAgentService;
        private readonly IAzureServiceBusListenerAgentService _azureServiceBusListenerAgentService;
        private readonly IRabbitMqListenerAgentService _rabbitMqListenerAgentService;
        private readonly IDebugLogsProcessingService _debugLogsProcessingService;
        private readonly IInstanceSync _instanceSync;
        private readonly Config _config;

        public BackgroundWorkingHostedService(
            ISchedulerAgentService schedulerAgentService,
            IBackgroundWorkerAgentService backgroundWorkerAgentService,
            IAzureServiceBusListenerAgentService azureServiceBusListenerAgentService,
            IRabbitMqListenerAgentService rabbitMqListenerAgentService,
            IDebugLogsProcessingService debugLogsProcessingService,
            IInstanceSync instanceSync,
            Config config)
        {
            _schedulerAgentService = schedulerAgentService;
            _backgroundWorkerAgentService = backgroundWorkerAgentService;
            _azureServiceBusListenerAgentService = azureServiceBusListenerAgentService;
            _rabbitMqListenerAgentService = rabbitMqListenerAgentService;
            _debugLogsProcessingService = debugLogsProcessingService;
            _instanceSync = instanceSync;
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _azureServiceBusListenerAgentService.ProcessTask();
                await _rabbitMqListenerAgentService.ProcessTask();
                await _debugLogsProcessingService.ProcessTask();
                if (_instanceSync.IsMainInstance)
                {
                    await _backgroundWorkerAgentService.ProcessTask();
                    await _schedulerAgentService.ProcessTask();
                }
                await Task.Run(AutoCompactLargeObjectHeap, cancellationToken);
                // Await all tasks to complete in parallel
                //await Task.WhenAll(backgroundWorkerTask, schedulerTask, azureServiceBusListenerTask, autoCompactTask);
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }

        private void AutoCompactLargeObjectHeap()
        {
            if (_config.AutoCompactLargeObjectHeap)
            {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}