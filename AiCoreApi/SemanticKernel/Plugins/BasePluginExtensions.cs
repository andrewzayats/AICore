using AiCoreApi.Models.DbModels;

namespace AiCoreApi.SemanticKernel.Plugins
{
    public static class BasePluginExtensions
    {
        public static bool SetValue(this Dictionary<string, ConfigurableSetting> configurableSettings, string code, string name, string value)
        {
            if (configurableSettings.ContainsKey(code))
            {
                configurableSettings[code].Value = value;
                return true;
            }
            configurableSettings.Add(code, new ConfigurableSetting
            {
                Code = code,
                Name = name,
                Value = value
            });
            return false;
        }
    }
}