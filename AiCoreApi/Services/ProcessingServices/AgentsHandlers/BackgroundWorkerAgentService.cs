using System.Web;
using AiCoreApi.Common;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using AiCoreApi.SemanticKernel.Agents;

namespace AiCoreApi.Services.ProcessingServices.AgentsHandlers
{
    public class BackgroundWorkerAgentService : IBackgroundWorkerAgentService
    {
        private readonly ISchedulerAgentTaskProcessor _schedulerAgentTaskProcessor;
        private readonly IAgentsProcessor _agentsProcessor; 
        private readonly IServiceProvider _serviceProvider;

        public BackgroundWorkerAgentService(
            ISchedulerAgentTaskProcessor schedulerAgentTaskProcessor,
            IAgentsProcessor agentsProcessor,
            ILoginProcessor loginProcessor,
            IServiceProvider serviceProvider)
        {
            _schedulerAgentTaskProcessor = schedulerAgentTaskProcessor;
            _agentsProcessor = agentsProcessor;
            _serviceProvider = serviceProvider;
        }


        public async Task ProcessTask()
        {
            var schedulerAgentTaskModel = await _schedulerAgentTaskProcessor.GetNext();
            while (schedulerAgentTaskModel != null)
            {
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    try
                    {
                        var userContextAccessor = scope.ServiceProvider.GetRequiredService<UserContextAccessor>();
                        var compositeAgent = scope.ServiceProvider.GetRequiredService<ICompositeAgent>();
                        var requestAccessor = scope.ServiceProvider.GetRequiredService<RequestAccessor>();
                        requestAccessor.SetRequestAccessor(schedulerAgentTaskModel.RequestAccessor);
                        userContextAccessor.SetLoginId(schedulerAgentTaskModel.LoginId);
                        UserContextAccessor.AsyncScheduledLoginId.Value = schedulerAgentTaskModel.LoginId;
                        schedulerAgentTaskModel.SchedulerAgentTaskState = SchedulerAgentTaskState.InProgress;
                        await _schedulerAgentTaskProcessor.Update(schedulerAgentTaskModel);
                        var compositeAgentModel = await _agentsProcessor.GetByName(schedulerAgentTaskModel.CompositeAgentName);
                        if (compositeAgentModel == null)
                        {
                            schedulerAgentTaskModel.Result = "Composite agent not found";
                            schedulerAgentTaskModel.SchedulerAgentTaskState = SchedulerAgentTaskState.Failed;
                            await _schedulerAgentTaskProcessor.Update(schedulerAgentTaskModel);
                            return;
                        }
                        var parameters = schedulerAgentTaskModel.Parameters.JsonGet<Dictionary<string, string>>();
                        var result = await compositeAgent.DoCall(compositeAgentModel, parameters, scope.ServiceProvider);
                        schedulerAgentTaskModel.Result = HttpUtility.HtmlDecode(result);
                        schedulerAgentTaskModel.SchedulerAgentTaskState = SchedulerAgentTaskState.Completed;
                        await _schedulerAgentTaskProcessor.Update(schedulerAgentTaskModel);
                    }
                    catch (Exception e)
                    {
                        schedulerAgentTaskModel.Result = e.Message;
                        schedulerAgentTaskModel.SchedulerAgentTaskState = SchedulerAgentTaskState.Failed;
                        await _schedulerAgentTaskProcessor.Update(schedulerAgentTaskModel);
                    }
                }
                schedulerAgentTaskModel = await _schedulerAgentTaskProcessor.GetNext();
            }
            await _schedulerAgentTaskProcessor.RemoveExpired();
        }
    }

    public interface IBackgroundWorkerAgentService
    {
        public Task ProcessTask();
    }
}