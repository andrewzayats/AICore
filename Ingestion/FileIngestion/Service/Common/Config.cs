namespace AiCore.FileIngestion.Service.Common;

public class Config
{
    public Config()
    {
        Proxy = GetValue<string>("Service__Proxy", "");
        IngestionTimeout = GetValue<string>("Service__IngestionTimeout", "00:10:00");
    }

    private T GetValue<T>(string key, string defaultValue)
    {
        var environmentValue = Environment.GetEnvironmentVariable(key) ?? defaultValue;
        return (T) Convert.ChangeType(environmentValue, typeof(T));
    }
    public string Proxy { get; set; }
    public string IngestionTimeout { get; set; }
}