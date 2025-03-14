using Microsoft.SemanticKernel;
using AiCoreApi.Models.DbModels;
using System.Web;
using AiCoreApi.Common;

namespace AiCoreApi.SemanticKernel.Agents
{
    public class IronPythonCodeAgent : BaseAgent, IIronPythonCodeAgent
    {
        private const string DebugMessageSenderName = "IronPythonCodeAgent";
        public static class AgentPromptPlaceholders
        {
            public const string HasFilesPlaceholder = "hasFiles";
            public const string FilesNamesPlaceholder = "filesNames";
            public const string FilesDataPlaceholder = "filesData";
        }

        private static class AgentContentParameters
        {
            public const string PythonCode = "pythonCode";
        }
        private readonly RequestAccessor _requestAccessor;
        private readonly ResponseAccessor _responseAccessor;

        public IronPythonCodeAgent(
            RequestAccessor requestAccessor,
            ResponseAccessor responseAccessor,
            ExtendedConfig extendedConfig,
            ILogger<IronPythonCodeAgent> logger) : base(requestAccessor, extendedConfig, logger)
        {
            _requestAccessor = requestAccessor;
            _responseAccessor = responseAccessor;
        }

        public override async Task<string> DoCall(
            AgentModel agent, 
            Dictionary<string, string> parameters)
        {
            parameters.ToList().ForEach(p => parameters[p.Key] = HttpUtility.HtmlDecode(p.Value));

            var pythonCode = ApplyParameters(agent.Content[AgentContentParameters.PythonCode].Value, parameters);
            pythonCode = ApplyParameters(pythonCode, new Dictionary<string, string>
            {
                {AgentPromptPlaceholders.HasFilesPlaceholder, _requestAccessor.MessageDialog.Messages.Last().HasFiles().ToString()},
                {AgentPromptPlaceholders.FilesDataPlaceholder, _requestAccessor.MessageDialog.Messages.Last().GetFileContents()},
                {AgentPromptPlaceholders.FilesNamesPlaceholder, _requestAccessor.MessageDialog.Messages.Last().GetFileNames()}
            });
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Execute Python Code", pythonCode);

            var eng = IronPython.Hosting.Python.CreateEngine();
            var scope = eng.CreateScope();
            scope.SetVariable("Parameters", parameters);
            scope.SetVariable("RequestAccessor", _requestAccessor);
            eng.Execute(pythonCode, scope);
            dynamic result = scope.GetVariable("result");
            _responseAccessor.AddDebugMessage(DebugMessageSenderName, "Python Code Result", result);
            return result;
        }
    }

    public interface IIronPythonCodeAgent
    {
        Task AddAgent(AgentModel agent, Kernel kernel, List<string> pluginsInstructions);
    }
}
