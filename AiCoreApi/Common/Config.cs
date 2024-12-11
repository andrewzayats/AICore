using AiCoreApi.Common.Extensions;
namespace AiCoreApi.Common;

public class Config
{
    public Config(): this(File.ReadAllText("appsettings.json")) { }

    public Config(string appSettings)
    {
        IsMainInstance = GetValue<bool>(appSettings, "IsMainInstance");
        var version = GetValue<string>(appSettings, "ProductVersion");
        ProductVersion = string.IsNullOrWhiteSpace(version) ? new Version(1, 0) : Version.Parse(version);
        DbConnectionTimeout = GetValue<int>(appSettings, "DbConnectionTimeout");
        DbServer = GetValue<string>(appSettings, "DbServer");
        DbPort = GetValue<int>(appSettings, "DbPort");
        DbName = GetValue<string>(appSettings, "DbName");
        DbUser = GetValue<string>(appSettings, "DbUser");
        DbPassword = GetValue<string>(appSettings, "DbPassword");
        DbTimeout = GetValue<int>(appSettings, "DbTimeout");
        DbPgPoolSize = GetValue<int>(appSettings, "DbPgPoolSize");
        AutoCompactLargeObjectHeap = GetValue<bool>(appSettings, "AutoCompactLargeObjectHeap");
    }

    private T GetValue<T>(string config, string key)
    {
        var environmentValue = Environment.GetEnvironmentVariable(key.ToUpper());
        if (environmentValue == null)
        {
            var value = config.JsonGet<T>(key);
            if (value != null)
                return value;
            throw new Exception($"Config key {key} not found");
        }
        return (T) Convert.ChangeType(environmentValue, typeof(T));
    }
    public bool IsMainInstance { get; set; }
    public Version ProductVersion { get; set; }
    public string DbServer { get; set; }
    public int DbPort { get; set; }
    public string DbName { get; set; }
    public string DbUser { get; set; }
    public string DbPassword { get; set; }
    public int DbTimeout { get; set; }
    public int DbPgPoolSize { get; set; }
    public int DbConnectionTimeout { get; set; }
    public bool AutoCompactLargeObjectHeap { get; set; }
}