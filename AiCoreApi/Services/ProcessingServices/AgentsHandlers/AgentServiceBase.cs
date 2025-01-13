using AiCoreApi.Common;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using AiCoreApi.SemanticKernel.Agents;

namespace AiCoreApi.Services.ProcessingServices.AgentsHandlers
{
    public class AgentServiceBase
    {
        private readonly ILoginProcessor _loginProcessor;
        protected readonly IServiceProvider ServiceProvider;

        public AgentServiceBase(
            ILoginProcessor loginProcessor,
            IServiceProvider serviceProvider)
        {
            _loginProcessor = loginProcessor;
            ServiceProvider = serviceProvider;
        }

        public async Task RunAgent(string sender, List<AgentModel> allAgents, AgentModel handlerAgent, string agentToCallName, int runAs, Dictionary<string, string> parametersValues)
        {
            var agentToCallModel = allAgents.FirstOrDefault(item => item.Name.ToLower() == agentToCallName.ToLower());
            if (agentToCallModel == null)
            {
                handlerAgent.Content["lastResult"].Value = $"Agent not found: {agentToCallName}";
            }
            else
            {
                var result = await RunAgent(sender, agentToCallModel, runAs, parametersValues);
                handlerAgent.Content["lastResult"].Value = result;
            }
            handlerAgent.Content["lastRun"].Value = DateTime.UtcNow.ToString("o");
        }

        public async Task<string> RunAgent(string sender, AgentModel agentToCallModel, int runAs, Dictionary<string, string> parametersValues)
        {
            try
            {
                var runAsUser = await _loginProcessor.GetById(runAs);
                if (runAsUser == null)
                    return "User not found";
                await using (var scope = ServiceProvider.CreateAsyncScope())
                {
                    var userContextAccessor = scope.ServiceProvider.GetRequiredService<UserContextAccessor>();
                    var requestAccessor = scope.ServiceProvider.GetRequiredService<RequestAccessor>();
                    requestAccessor.MessageDialog = new Models.ViewModels.MessageDialogViewModel
                    {
                        Messages = new List<Models.ViewModels.MessageDialogViewModel.Message>
                        {
                            new()
                            {
                                Text = $"{sender} task",
                                Sender = sender,
                            }
                        }
                    };
                    // Set user context with all tags for scheduled task
                    requestAccessor.Login = runAsUser.Login;
                    requestAccessor.LoginTypeString = runAsUser.LoginType.ToString();
                    requestAccessor.TagsString = string.Join(",", runAsUser.Tags.Select(tag => tag.TagId));
                    userContextAccessor.SetLoginId(runAs);
                    UserContextAccessor.AsyncScheduledLoginId.Value = runAs;
                    var result = "";
                    if (agentToCallModel.Type == AgentType.Composite)
                    {
                        var compositeAgent = scope.ServiceProvider.GetRequiredService<ICompositeAgent>();
                        result = await compositeAgent.DoCall(agentToCallModel, parametersValues, scope.ServiceProvider);
                    }
                    else if (agentToCallModel.Type == AgentType.CsharpCode)
                    {
                        var csharpCodeAgent = scope.ServiceProvider.GetRequiredService<ICsharpCodeAgent>();
                        result = await csharpCodeAgent.DoCall(agentToCallModel, parametersValues);
                    }
                    else if (agentToCallModel.Type == AgentType.PythonCode)
                    {
                        var pythonCodeAgent = scope.ServiceProvider.GetRequiredService<IPythonCodeAgent>();
                        result = await pythonCodeAgent.DoCall(agentToCallModel, parametersValues);
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }
    }
}