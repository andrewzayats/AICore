using AiCoreApi.Common;

namespace AiCoreApi.Services.UpdateServices;

public class UpdateService
{
    private readonly Config _config;
    private readonly ILogger<UpdateService> _logger;
    private UpdateDatabaseService _dbUpdate;
    private IHost _host;

    public UpdateService(string[] args)
    {
        _config = new Config();
        _host = Host.CreateDefaultBuilder(args).Build();
        _logger = _host.Services.GetRequiredService<ILogger<UpdateService>>();
        _dbUpdate = new UpdateDatabaseService(_config, _host);
    }

    /// <summary>
    /// Check whether update is available or not
    /// IF available - check is can be updated (this instance is main)
    ///     Instance is main - run update
    ///     Instance is not main - wait till another instance update all things
    /// IF not available - return
    /// </summary>
    public void Update()
    {
        try
        {
            if (!IsUpdateRequired()) return;

            if (_config.IsMainInstance)
            {
                _dbUpdate.UpdateDatabase();
                _dbUpdate.SetProductVersion(_config.ProductVersion);
            }
            else
            {
                // Wait until some other main instance
                // will update what is necessary
                while (IsUpdateRequired())
                {
                    Thread.Sleep(5000);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }

    private bool IsUpdateRequired()
    {
        return _dbUpdate.IsUpdateRequired();
    }
}