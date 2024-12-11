using AiCoreApi.Models.ViewModels;
using AiCoreApi.SemanticKernel;

namespace AiCoreApi.Common
{
    public class ResponseAccessor
    {
        private readonly ILogger<ResponseAccessor> _logger;
        private readonly RequestAccessor _requestAccessor;
        public ResponseAccessor(
            ILogger<ResponseAccessor> logger,
            RequestAccessor requestAccessor)
        {
            _logger = logger;
            _requestAccessor = requestAccessor;
        }
        public string? StepState { get; set; }
        public MessageDialogViewModel.Message CurrentMessage { get; set; } = new() { Sender = PlannerHelpers.AssistantName };
        public void AddDebugMessage(string sender, string title, string details)
        {
            if (_requestAccessor.UseDebug)
            {
                CurrentMessage.DebugMessages ??= new List<MessageDialogViewModel.DebugMessage>();
                CurrentMessage.DebugMessages.Add(new MessageDialogViewModel.DebugMessage
                {
                    Sender = sender,
                    Title = title,
                    Details = details,
                    DateTime = DateTime.UtcNow
                });
            }
            _logger.LogDebug($"{4}, {0}: {1}, {2}", sender, title, details, _requestAccessor.Login);
        }

        public void AddSpentTokens(string modelName, int requestTokens, int responseTokens)
        {
            CurrentMessage.SpentTokens ??= new Dictionary<string, MessageDialogViewModel.TokensSpent>();
            if (!CurrentMessage.SpentTokens.ContainsKey(modelName))
                CurrentMessage.SpentTokens[modelName] = new MessageDialogViewModel.TokensSpent();
            CurrentMessage.SpentTokens[modelName].Request += requestTokens;
            CurrentMessage.SpentTokens[modelName].Response += responseTokens;
        }
    }
}
