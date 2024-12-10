namespace AiCoreApi.Common
{
    public class FeatureFlags: IFeatureFlags
    {
        public class Names
        {
            public const string Tagging = "TAGGING";
        }

        private const string FeaturePrefix = "FEATURE_";
        private readonly Dictionary<string, bool> _configValues = new();

        public FeatureFlags()
        {
            Environment.GetEnvironmentVariables().Keys.Cast<string>().ToList().ForEach(key =>
            {
                var keyUpper = key.ToUpper();
                if (keyUpper.StartsWith(FeaturePrefix))
                {
                    _configValues.TryAdd(keyUpper.Replace(FeaturePrefix, string.Empty), Convert.ToBoolean(Environment.GetEnvironmentVariable(key)));
                }
            });
        }

        public Dictionary<string, bool> GetValues() => _configValues;
        public bool IsEnabled(string key) => _configValues.TryGetValue(key.ToUpper(), out var value) && value;
    }

    public interface IFeatureFlags
    {
        Dictionary<string, bool> GetValues();
        bool IsEnabled(string key);
    }
}

