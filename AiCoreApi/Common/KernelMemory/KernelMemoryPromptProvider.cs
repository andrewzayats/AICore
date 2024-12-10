using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Prompts;

namespace AiCoreApi.Common.KernelMemory
{
    public class MarkdownPromptProvider : IPromptProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EmbeddedPromptProvider _fallbackProvider = new();

        public MarkdownPromptProvider(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string ReadPrompt(string promptName)
        {
            var httpContext = _serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
            var serviceProvider = httpContext?.RequestServices ?? _serviceProvider;

            var responseAccessor = serviceProvider.GetService<ResponseAccessor>();

            switch (promptName)
            {
                case Constants.PromptNamesAnswerWithFacts:
                    {
                        return responseAccessor?.StepState ?? _fallbackProvider.ReadPrompt(promptName);
                    }
                default:
                    // Fall back to the default
                    return _fallbackProvider.ReadPrompt(promptName);
            }
        }
    }
}