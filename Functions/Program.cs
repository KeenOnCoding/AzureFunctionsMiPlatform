using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         config.AddJsonFile("host.json", true);
         config.AddJsonFile("local.settings.json", true);
         config.AddEnvironmentVariables();

         if (args != null)
             config.AddCommandLine(args);
     })
    .ConfigureServices(service =>
    {
            service.Configure<LoggerFilterOptions>(options =>
            {
                // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override. Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
                var toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

                if (toRemove is not null)
                {
                    options.Rules.Remove(toRemove);
                }
            });
    })
    //.ConfigureFunctionsWorkerDefaults(builder => { }, options => { })
    .Build();

host.Run();
