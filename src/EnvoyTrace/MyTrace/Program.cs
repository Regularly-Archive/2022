using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Text;
using System.Diagnostics.Tracing;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry;

namespace MyTrace
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddScoped<MyTraceInvoker>();
            services.AddSingleton(sp => new ActivitySource("MyTrace"));
            services.AddScoped(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("MyActivityListener");
                return new ActivityListener()
                {
                    ShouldListenTo = _ => true,
                    Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
                    ActivityStopped = activity =>
                    {
                        //logger.LogInformation($"{activity.DisplayName},ParentSpanId={activity.ParentSpanId},TraceId={activity.TraceId},SpanId={activity.SpanId},Duration={activity.Duration}");

                        //var stringBuilder = new StringBuilder();

                        //foreach (var kv in activity.TagObjects)
                        //{
                        //    stringBuilder.AppendLine($"{kv.Key}={kv.Value}");
                        //}

                        //if (stringBuilder.Length > 0)
                        //    logger.LogInformation($"TagObjects:{stringBuilder.ToString()}");
                    }
                };
            });
            services.AddLogging(option =>
            {
                option.Configure(x => x.ActivityTrackingOptions = ActivityTrackingOptions.SpanId | ActivityTrackingOptions.TraceId);
                option.SetMinimumLevel(LogLevel.Debug);
                option.AddConsole().AddNLog("NLog.config");
            });

            Sdk.CreateTracerProviderBuilder()
            .AddSource("MyTrace")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Program"))
            .AddConsoleExporter()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "192.168.50.162";
                options.AgentPort = 6831;
            })
            .Build();


            var serviceProvider = services.BuildServiceProvider();


            var activityListener = serviceProvider.GetRequiredService<ActivityListener>();
            ActivitySource.AddActivityListener(activityListener);

            var invoker = serviceProvider.GetRequiredService<MyTraceInvoker>();
            var result = await invoker.InvokeAsync();

            Console.ReadKey();
        }
    }
}
