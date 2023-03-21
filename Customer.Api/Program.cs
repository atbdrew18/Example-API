using Azure.Identity;
using Customer.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace OMNIMED.CustomerCloud.CloudApi
{
    /// <summary>
    /// Main application entry point class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the application configuration
        /// </summary>
        public static IConfiguration _configuration { get; set; }

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            SetupConfiguration(args);
            SetupLogging();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Create Host Builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //
            // Build the host
            //
            return new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseConfiguration(_configuration)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>();
                });
        }

        /// <summary>
        /// SetupConfiguration
        /// </summary>
        /// <param name="args"></param>
        public static void SetupConfiguration(string[] args)
        {
            //
            // Setup application configuraton
            //
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "production";
            var isDevelopment = environmentName.ToLower() == "development";

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            _configuration = configurationBuilder.Build();

            //
            // This will add azure keyvault support if the application is configured with the approporiate keyvault properties
            //
            AddAzureKeyVaultSupport(configurationBuilder);
        }

        /// <summary>
        /// AddAzureKeyVaultSupport
        /// </summary>
        /// <param name="configurationBuilder"></param>
        public static void AddAzureKeyVaultSupport(IConfigurationBuilder configurationBuilder)
        {
            var keyVaultUrl = _configuration["KeyVault:Url"];
            if (!string.IsNullOrWhiteSpace(keyVaultUrl))
            {
                //TODO Need to test this works
                configurationBuilder.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
            }
        }

        /// <summary>
        /// Setup the logging approach for the application
        /// </summary>
        public static void SetupLogging()
        {
            //
            // Configure Serilog. Taken from example at: https://github.com/serilog/serilog-aspnetcore/blob/dev/samples/EarlyInitializationSample/Program.cs
            //
            var logFileName = _configuration["Serilog:LogFileName"];
            //var outputTemplateString = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{MachineName}: {EnvironmentUserName}] {Message:lj}{Properties:j}{NewLine}{Exception}";
            var outputTemplateString = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{MachineName}: {EnvironmentUserName}] {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.FromLogContext()
                .WriteTo.File(logFileName, LogEventLevel.Verbose, outputTemplate: outputTemplateString, rollingInterval: RollingInterval.Day)
                .WriteTo.Console(LogEventLevel.Verbose, outputTemplate: outputTemplateString)
                //.WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
                .CreateLogger();
        }
    }
}
