namespace AiCore.FileIngestion.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var startTime = DateTimeOffset.UtcNow;
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup();
        startup.ConfigureServices(builder.Services);
        var app = builder.Build();
        startup.Configure(app, builder.Environment, startTime);
        app.Run();
    }
}