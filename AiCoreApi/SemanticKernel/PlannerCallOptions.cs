using System.Text;
using AiCoreApi.Common;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Common.Extensions;
using AiCoreApi.Models.DbModels;
using AiCoreApi.SemanticKernel.Agents;
using System.Net;

namespace AiCoreApi.SemanticKernel
{
    public class PlannerCallOptions : IPlannerCallOptions
    {
        private const string DebugMessageSenderName = "PlannerCallOptions";

        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;


        public PlannerCallOptions(
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor)
        {
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor;
        }

        public async Task<ApplyCallOptionsResult> Apply(List<AgentModel> agentsList)
        {
            var result = new ApplyCallOptionsResult{Success = true};
            var currentMessage = _requestAccessor.MessageDialog?.Messages?.Last();
            if (currentMessage?.Options == null || currentMessage.Options.Length == 0)
                return result;
            foreach (var option in currentMessage.Options)
            {
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Call Option", option.ToJson());
                // Agent direct call
                if (option.Type == MessageDialogViewModel.CallOptions.CallOptionsType.AgentCall)
                {
                    await ApplyAgentCall(option, result, agentsList);
                    if (!result.Success)
                        return result;
                }
            }
            return result;
        }

        private async Task ApplyAgentCall(MessageDialogViewModel.CallOptions option, ApplyCallOptionsResult result, List<AgentModel> agentsList)
        {
            var agentName = option.Name.ToLower();
            var agentParameters = option.Parameters;
            var agent = agentsList.FirstOrDefault(agent => agent.Name.ToLower() == agentName);
            if (agent == null)
            {
                var availableAgentNames = agentsList.Select(agent => agent.Name.ToLower()).ToList();
                _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Agent not found", $"Required: {agentName}, Found: {string.Join(", ", availableAgentNames)}");
                result.ErrorMessage = $"Agent '{agentName}' not found.";
                result.Success = false;
                return;
            }
            // Generate Handlebars plan for agent call
            var planStringBuilder = new StringBuilder();
            planStringBuilder.AppendLine("{{!-- Step 0: Extract key values --}}");
            foreach (var parameter in agentParameters)
            {
                planStringBuilder.AppendLine($"{{{{set \"{parameter.Key}Value\" \"{WebUtility.HtmlEncode(parameter.Value)}\"}}}}");
            }
            var parameterNames = string.Join(" ", agentParameters.Select(parameter => $"{parameter.Key}={parameter.Key}Value").ToList());
            var functionName = GetFullFunctionName(agent);
            planStringBuilder.AppendLine();
            planStringBuilder.AppendLine("{{!-- Step 1: Call the agent --}}");
            planStringBuilder.AppendLine($"{{{{set \"agentResult\" ({functionName} {parameterNames})}}}}");
            planStringBuilder.AppendLine();
            planStringBuilder.AppendLine("{{!-- Step 2: Output the result --}}");
            planStringBuilder.AppendLine("{{json agentResult}}");
            result.Plan = planStringBuilder.ToString();
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Call Plan", result.Plan);
        }

        private string GetFullFunctionName(AgentModel agent)
        {
            var functionName = agent.Name.ToCamelCase();
            return $"{functionName}Plugin-{functionName}";
        }

        public class ApplyCallOptionsResult
        {
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
            public string? Plan { get; set; }
        }
    }

    public interface IPlannerCallOptions
    {
        Task<PlannerCallOptions.ApplyCallOptionsResult> Apply(List<AgentModel> agentsList);
    }
}
