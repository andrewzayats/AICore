using System.Runtime;
using System.Web;
using AiCoreApi.Common;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using AiCoreApi.SemanticKernel.Agents;
using Microsoft.Extensions.Caching.Distributed;
using NCrontab;

namespace AiCoreApi.Services.ProcessingServices
{
    public class BackgroundWorkingHostedService : IHostedService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ISchedulerAgentTaskProcessor _schedulerAgentTaskProcessor;
        private readonly ILoginProcessor _loginProcessor;
        private readonly IAgentsProcessor _agentsProcessor; 
        private readonly IServiceProvider _serviceProvider;
        private readonly Config _config;

        public BackgroundWorkingHostedService(
            IDistributedCache distributedCache,
            ISchedulerAgentTaskProcessor schedulerAgentTaskProcessor,
            ILoginProcessor loginProcessor,
            IAgentsProcessor agentsProcessor,
            IServiceProvider serviceProvider,
            Config config)
        {
            _distributedCache = distributedCache;
            _schedulerAgentTaskProcessor = schedulerAgentTaskProcessor;
            _loginProcessor = loginProcessor;
            _agentsProcessor = agentsProcessor;
            _serviceProvider = serviceProvider;
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var backgroundWorkerTask = ProcessBackgroundWorkerTask();
                var schedulerTask = ProcessSchedulerTask();
                var autoCompactTask = Task.Run(AutoCompactLargeObjectHeap, cancellationToken);
                // Await all tasks to complete in parallel
                await Task.WhenAll(backgroundWorkerTask, schedulerTask, autoCompactTask);
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }

        private async Task ProcessBackgroundWorkerTask()
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

        private async Task ProcessSchedulerTask()
        {
            var agents = await _agentsProcessor.List();
            var schedulerAgents = agents.Where(agent => agent.Type == AgentType.Scheduler).ToList();
            foreach (var agent in schedulerAgents)
            {
                if(!agent.IsEnabled)
                    continue;
                var cronScheduleStartTime = agent.Content["schedule"].Value;
                var runAs = Convert.ToInt32(agent.Content["runAs"].Value);
                var compositeAgentName = agent.Content["compositeAgentName"].Value;
                var parametersValues = (agent.Content.ContainsKey("parametersValues") ? agent.Content["parametersValues"].Value : "")
                    .Split(',')
                    .Select((value, index) => new { Key = "parameter" + (index + 1), Value = value })
                    .ToDictionary(item => item.Key, item => item.Value);
                var lastRun = DateTime.TryParse(agent.Content.ContainsKey("lastRun") ? agent.Content["lastRun"].Value : "", out var lastRunDateTime);
                if(!lastRun)
                    lastRunDateTime = DateTime.MinValue;

                var cronExpression = CrontabSchedule.Parse(cronScheduleStartTime);
                var nextRunTime = cronExpression.GetNextOccurrence(lastRunDateTime);
                if (DateTime.UtcNow >= nextRunTime)
                {
                    var compositeAgent = agents.FirstOrDefault(item => item.Name.ToLower() == compositeAgentName.ToLower());
                    if (compositeAgent == null)
                    {
                        agent.Content["lastResult"].Value = $"Composite agent not found: {compositeAgentName}";
                    }
                    else
                    {
                        var result = await RunScheduledTask(compositeAgent, runAs, parametersValues);
                        agent.Content["lastResult"].Value = result;
                    }

                    agent.Content["lastRun"].Value = DateTime.UtcNow.ToString("o");
                    await _agentsProcessor.Update(agent);
                }
            }
        }

        private async Task<string> RunScheduledTask(AgentModel agent, int runAs, Dictionary<string, string> parametersValues)
        {
            try
            {
                var runAsUser = await _loginProcessor.GetById(runAs);
                if (runAsUser == null)
                    return "User not found";
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var userContextAccessor = scope.ServiceProvider.GetRequiredService<UserContextAccessor>();
                    var compositeAgent = scope.ServiceProvider.GetRequiredService<ICompositeAgent>();
                    var requestAccessor = scope.ServiceProvider.GetRequiredService<RequestAccessor>();
                    requestAccessor.MessageDialog = new Models.ViewModels.MessageDialogViewModel
                    {
                        Messages = new List<Models.ViewModels.MessageDialogViewModel.Message>
                        {
                            new()
                            {
                                Text = "Scheduled task",
                                Sender = "Scheduler",
                            }
                        }
                    };
                    // Set user context with all tags for scheduled task
                    requestAccessor.Login = runAsUser.Login;
                    requestAccessor.LoginTypeString = runAsUser.LoginType.ToString();
                    requestAccessor.TagsString = string.Join(",", runAsUser.Tags.Select(tag => tag.TagId)); 
                    userContextAccessor.SetLoginId(runAs);
                    UserContextAccessor.AsyncScheduledLoginId.Value = runAs;
                    var result = await compositeAgent.DoCall(agent, parametersValues, scope.ServiceProvider);
                    return result;
                }
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
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

        public static string JiraIntegrationServicePrepareReportCacheKey => "JiraIntegrationService|PrepareReportAsync";

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}