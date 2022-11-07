using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using OrderService.Extensions;
using OrderService;

namespace OrderService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(option =>
            {
                option.AddNLog();
            });
            services.AddControllers();
            services.AddHeaderPropagation(opt =>
            {
                opt.Headers.Add("x-request-id");
                opt.Headers.Add("x-b3-traceid");
                opt.Headers.Add("x-b3-spanid");
                opt.Headers.Add("x-b3-parentspanid");
                opt.Headers.Add("x-b3-sampled");
                opt.Headers.Add("x-b3-flags");
                opt.Headers.Add("x-ot-span-context");
            });
            services.AddHttpClient("PaymentService", client =>
            {
                client.BaseAddress = new Uri("http://192.168.50.162:9090");
            })
            .AddHeaderPropagation();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService", Version = "v1" });
            });

            services.AddOpenTelemetryTracing((builder) => builder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("reverse-proxy"))
            //.AddAspNetCoreInstrumentation()
            //.AddHttpClientInstrumentation()
            .AddSource("MyTrace")
            .AddSource("OrderService")
            .AddConsoleExporter()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "192.168.50.162";
                options.AgentPort = 6831;
            }));

            DiagnosticListener instance = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton(instance);

            services.AddTransient<MyDiagnosticObserver>();
            services.AddSingleton<MyTraceInvoker>(sp =>
            {
                return new MyTraceInvoker(sp.GetService<ILoggerFactory>().CreateLogger<MyTraceInvoker>());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var listener = app.ApplicationServices.GetService<DiagnosticListener>();
            var observer = app.ApplicationServices.GetService<MyDiagnosticObserver>();
            listener.Subscribe(observer);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService v1"));

            app.UseHttpsRedirection();

            app.UseHeaderPropagation();

            app.UseMiddleware<TraceMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
