using System.Net;
using System.Reflection;
using System.Text;
using AiCore.FileIngestion.Service.Common;
using AiCore.FileIngestion.Service.Common.Extensions;
using Polly;
using Polly.Extensions.Http;

namespace AiCore.FileIngestion.Service
{
    public class Startup
    {
        private readonly Config _config;
        private static ILogger<Startup> _logger;

        public Startup()
        {
            _config = new Config();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });
            services.AddSingleton(_config);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton<SelfDestructionMiddleware>();
            services.AddHealthChecks();
            services.ForInterfacesMatching("^I").OfAssemblies(Assembly.GetExecutingAssembly()).AddTransients();
            var serviceProvider = services.BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
            services.AddHttpClient("RetryClient", httpClient =>
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(2);
                })
                .AddPolicyHandler(GetRetryPolicy())
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    Proxy = string.IsNullOrEmpty(_config.Proxy) ? null : new WebProxy(new Uri(_config.Proxy)),
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });

            services.AddHealthChecks();
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddLogging(c => c.AddConsole(opt => opt.LogToStandardErrorThreshold = LogLevel.Debug));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void Configure(WebApplication app, IWebHostEnvironment env, DateTimeOffset startTime)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseWhen(IsIngestionEndpoint, appBuilder =>
            {
                var middleware = appBuilder.ApplicationServices.GetRequiredService<SelfDestructionMiddleware>();
                appBuilder.Use(middleware.Invoke);
            });

            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        private static bool IsIngestionEndpoint(HttpContext context) => context.Request.Method == HttpMethods.Post && context.Request.Path.ToString().Contains("/files");

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg =>
            {
                var nonSuccessRequest =
                    msg.StatusCode != HttpStatusCode.OK &&
                    msg.StatusCode != HttpStatusCode.Accepted &&
                    msg.StatusCode != (HttpStatusCode)424 &&
                    msg.StatusCode != HttpStatusCode.NoContent;
                if (nonSuccessRequest)
                {
                    _logger.LogTrace("Startup: {0}, url: {1}, request headers: {2}, code: {3}, body: {4}, response headers: {5}", "GetRetryPolicy",
                        msg.RequestMessage.RequestUri, msg.RequestMessage.Headers, msg.StatusCode, msg.Content.ReadAsStringAsync().Result, msg.Headers);
                }
                return nonSuccessRequest;
            })
            .WaitAndRetryAsync(7, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
