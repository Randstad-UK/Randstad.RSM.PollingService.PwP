using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.Models;
using Randstad.RSM.PollingService.PwP.Services.Api;
using Randstad.RSM.PollingService.PwP.Services.DataAccess;
using Randstad.RSM.PollingService.PwP.Services.FileConversion;
using Randstad.RSM.PollingService.PwP.Services.Import;
using Randstad.RSM.PollingService.PwP.Settings;
using Randstad.RSM.PollingService.PwP.Template.Application;
using Randstad.RSM.PollingService.PwP.Template.Extensions;
using RandstadMessageExchange;
using ServiceDiscovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Randstad.RSM.PollingService.PwP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    // extending default shutdown timeout on cancellation token from 5 to 10 seconds
                    services.Configure<HostOptions>(option => { option.ShutdownTimeout = TimeSpan.FromSeconds(10); });

                    // get configurations
                    services.Configure<ApplicationSettings>(hostContext.Configuration.GetSection(Constants.ApplicationConfigHeader));

                    // configuration IoC
                    services.AddSingleton(_ => _.GetRequiredService<IOptions<ApplicationSettings>>().Value);
                    services.AddSingleton(_ => _.GetRequiredService<IOptions<ServiceDiscoverySettings>>().Value);
                    services.AddSingleton(_ => _.GetRequiredService<IOptions<LoggingSettings>>().Value);
                    services.AddSingleton(_ => _.GetRequiredService<IOptions<MonitoringSettings>>().Value);
                    services.AddSingleton(_ => _.GetRequiredService<IOptions<ClientApiSettings>>().Value);

                    // services IoC
                    var applicationSettings = new ApplicationSettings();
                    var customSettings = new CustomSettings();
                    var loggingSettings = new LoggingSettings();
                    var serviceDiscoverySettings = new ServiceDiscoverySettings();
                    var monitoringSettings = new MonitoringSettings();
                    hostContext.Configuration.GetSection(Constants.ApplicationConfigHeader).Bind(applicationSettings);
                    var serviceDetailsSettings = serviceDiscoverySettings.ServiceDetails;
                    var sdClient = new ServiceDiscoveryClient(new List<string> { serviceDiscoverySettings.BaseUrl },
                        serviceDiscoverySettings.ServiceDetails.Name,
                        serviceDiscoverySettings.ServiceDetails.HostServer,
                        applicationSettings.Environment,
                        RequiredConfigurationGroupNames: null,
                        serviceDetailsSettings.IsApi,
                        serviceDetailsSettings.BaseUrl,
                        serviceDetailsSettings.AllowMonitorRestart);
                    var rabbitSettingsForLogger = sdClient.GetConfigurationGroup_RabbitMQSettings().ForLogger();

                    var retentionPolicy = new RetentionPolicy
                    {
                        RetentionPeriodInDaysDebug = loggingSettings.RetentionPeriodInDaysDebug,
                        RetentionPeriodInDaysError = loggingSettings.RetentionPeriodInDaysError,
                        RetentionPeriodInDaysFatal = loggingSettings.RetentionPeriodInDaysFatal,
                        RetentionPeriodInDaysInfo = loggingSettings.RetentionPeriodInDaysInfo,
                        RetentionPeriodInDaysSuccess = loggingSettings.RetentionPeriodInDaysSuccess,
                        RetentionPeriodInDaysWarn = loggingSettings.RetentionPeriodInDaysWarn
                    };
                    var assemblyName = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName();
                    var systemVersion = $"{assemblyName.Version}";
                    var logger = Logger.BuildLogger(
                        loggingSettings.LogLevel,
                        applicationSettings.Environment,
                        applicationSettings.ServiceName,
                        systemVersion,
                        loggingSettings.OperatingCompany,
                        rabbitSettingsForLogger,
                        retentionPolicy);
                    var producerSettings = sdClient.GetConfigurationGroup_RabbitMQ();

                    using (var monitorConsumerService = new ConsumerService(
                        producerSettings,
                        monitoringSettings.QueueName,
                        new List<string> { monitoringSettings.RoutingKey }))
                    {
                        monitorConsumerService.CreateQueueAndBindings();
                    }

                    // services IoC
                    services.AddHostedService<MessageProducerService>();
                    services.AddSingleton<ILogger>(_ => logger);
                    services.AddSingleton<IServiceDiscoveryClient>(_ => sdClient);
                    services.AddSingleton<IProducerService>(_ => new ProducerService(producerSettings));
                    services.AddSingleton<IErrorHandler, ErrorHandler>();
                    services.AddSingleton<IMessageProducer, MessageProducer>();
                    services.AddScoped<IDataAccess>(_ =>
                    {
                        var connectionString =
                            sdClient.GetConnectionString(serviceDetailsSettings.Name,
                                $"{applicationSettings.Environment}.{serviceDetailsSettings.Name}");
                        return new SqlDataAccess(connectionString);
                    });
                    services.AddScoped<IImportService, ImportService>();
                    services.AddHttpClient<IApiService, ApiService>(httpClient =>
                    {
                        var clientApiSettings = hostContext.Configuration.GetSection(Constants.RSMApiHeader).Get<ClientApiSettings>();
                        var baseUrl = sdClient.GetService(clientApiSettings.Name, applicationSettings.Environment).FirstOrDefault()?.BaseUrl;
                        httpClient.BaseAddress = new Uri(baseUrl ?? string.Empty);
                    });

                    logger.Info("ConfigureServices done.", Guid.NewGuid(), null, null, null, null);
                });
    }
}
