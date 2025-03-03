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
        protected readonly IServiceProvider ServiceProvider;

        public BackgroundWorkerAgentService(
            ISchedulerAgentTaskProcessor schedulerAgentTaskProcessor,
            IAgentsProcessor agentsProcessor,
            IServiceProvider serviceProvider)
        {
            _schedulerAgentTaskProcessor = schedulerAgentTaskProcessor;
            _agentsProcessor = agentsProcessor;
            _serviceProvider = serviceProvider;
            ServiceProvider = serviceProvider;
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
                        var requestAccessor = scope.ServiceProvider.GetRequiredService<RequestAccessor>();
                        requestAccessor.SetRequestAccessor(schedulerAgentTaskModel.RequestAccessor);
                        userContextAccessor.SetLoginId(schedulerAgentTaskModel.LoginId);
                        UserContextAccessor.AsyncScheduledLoginId.Value = schedulerAgentTaskModel.LoginId;
                        schedulerAgentTaskModel.SchedulerAgentTaskState = SchedulerAgentTaskState.InProgress;
                        await _schedulerAgentTaskProcessor.Update(schedulerAgentTaskModel);
                        var agentToCallModel = await _agentsProcessor.GetByName(schedulerAgentTaskModel.CompositeAgentName);
                        if (agentToCallModel == null)
                        {
                            schedulerAgentTaskModel.Result = "Agent to call not found";
                            schedulerAgentTaskModel.SchedulerAgentTaskState = SchedulerAgentTaskState.Failed;
                            await _schedulerAgentTaskProcessor.Update(schedulerAgentTaskModel);
                            return;
                        }
                        var parameters = schedulerAgentTaskModel.Parameters.JsonGet<Dictionary<string, string>>();
                        var result = string.Empty;
                        if (agentToCallModel.Type == AgentType.Composite)
                        {
                            var compositeAgent = scope.ServiceProvider.GetRequiredService<ICompositeAgent>();
                            result = await compositeAgent.DoCall(agentToCallModel, parameters, scope.ServiceProvider);
                        }
                        else if (agentToCallModel.Type == AgentType.CsharpCode)
                        {
                            var csharpCodeAgent = scope.ServiceProvider.GetRequiredService<ICsharpCodeAgent>();
                            result = await csharpCodeAgent.DoCall(agentToCallModel, parameters);
                        }
                        else if (agentToCallModel.Type == AgentType.PythonCode)
                        {
                            var pythonCodeAgent = scope.ServiceProvider.GetRequiredService<IPythonCodeAgent>();
                            result = await pythonCodeAgent.DoCall(agentToCallModel, parameters);
                        }
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