using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace GenericHostDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Trace()
                .WriteTo.Debug();

            Log.Logger = logConfig
                .Enrich.FromLogContext().CreateLogger();

            try
            {
                var webHost = CreateWebHostBuilder(args).Build();
                var genericHost = CreateHostBuilder(args).Build();

                await Task.WhenAll(
                    genericHost.RunAsync()
                    ,
                    webHost.RunAsync()
                );
            }
            //catch (OperationCanceledException)
            //{
            //    //If only have Generic Host, Press Ctrl+C shutdown will trigger OperationCanceledException
            //    Log.Information("Server is shutting down");
            //}
            catch (Exception ex)
            {
                Log.Fatal(ex, "web site terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder().ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("hostsettings.json", optional: true)
                        .AddEnvironmentVariables(prefix: "HOSTPREFIX_")
                        .AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true)
                        .AddEnvironmentVariables(prefix: "MYPREFIX_")
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddHostedService<HostedServices.LifetimeEventsHostedService>()
                        .AddHostedService<HostedServices.TimedHostedService>();
                })
                .UseConsoleLifetime()
                .UseSerilog();
    }
}
