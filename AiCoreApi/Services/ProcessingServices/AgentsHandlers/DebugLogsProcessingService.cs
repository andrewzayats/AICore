using AiCoreApi.Common;
using AiCoreApi.Data.Processors;

namespace AiCoreApi.Services.ProcessingServices.AgentsHandlers
{
    public class DebugLogsProcessingService : IDebugLogsProcessingService
    {
        private readonly IDebugLogProcessor _debugLogProcessor;
        private readonly ExtendedConfig _extendedConfig;

        public DebugLogsProcessingService(
            ExtendedConfig extendedConfig,
            IDebugLogProcessor debugLogProcessor)
        {
            _extendedConfig = extendedConfig;
            _debugLogProcessor = debugLogProcessor;
        }

        public async Task ProcessTask()
        {
            if (_extendedConfig.AllowDebugMode)
            {
                await _debugLogProcessor.Remove(DateTime.UtcNow.AddDays(-_extendedConfig.DebugMessagesStoreDays));
            }
        }
    }

    public interface IDebugLogsProcessingService
    {
        public Task ProcessTask();
    }
}