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
        private readonly Config _config;

        public BackgroundWorkingHostedService(
            ISchedulerAgentService schedulerAgentService,
            IBackgroundWorkerAgentService backgroundWorkerAgentService,
            IAzureServiceBusListenerAgentService azureServiceBusListenerAgentService,
            Config config)
        {
            _schedulerAgentService = schedulerAgentService;
            _backgroundWorkerAgentService = backgroundWorkerAgentService;
            _azureServiceBusListenerAgentService = azureServiceBusListenerAgentService;
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _backgroundWorkerAgentService.ProcessTask();
                await _schedulerAgentService.ProcessTask();
                await _azureServiceBusListenerAgentService.ProcessTask();
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