using Microsoft.Extensions.Hosting;
using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.Settings;
using Randstad.RSM.PollingService.PwP.Template.Settings;
using ServiceDiscovery;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Template.Application
{
    internal class MessageProducerService : BackgroundService
    {
        private const int DelayBeforeKillInSeconds = 10;

        private readonly ILogger _logger;
        private readonly IServiceDiscoveryClient _serviceDiscoveryClient;
        private readonly string _serviceName;
        private readonly int _maxPollingIntervalInSeconds;
        private readonly int _pollingIntervalIncrementInSeconds;
        private readonly int _initialPollingIntervalInSeconds;
        private readonly IErrorHandler _errorHandler;
        private readonly IMessageProducer _messageProducer;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private int _delay;
        private bool _removeFromSd;
        private Guid _correlationId;

        public MessageProducerService(
            ILogger logger,
            IServiceDiscoveryClient serviceDiscoveryClient,
            ApplicationSettings applicationSettings,
            IErrorHandler errorHandler,
            IHostApplicationLifetime hostApplicationLifetime,
            IMessageProducer messageProducer)
        {
            _logger = logger;
            _serviceDiscoveryClient = serviceDiscoveryClient;
            _serviceName = applicationSettings.ServiceName;
            _maxPollingIntervalInSeconds = applicationSettings.MaxPollingIntervalInSeconds;
            _pollingIntervalIncrementInSeconds = applicationSettings.PollingIntervalIncrementInSeconds;
            _initialPollingIntervalInSeconds = applicationSettings.PollingIntervalInSeconds;
            _delay = _initialPollingIntervalInSeconds;
            _removeFromSd = true;
            _hostApplicationLifetime = hostApplicationLifetime;
            _errorHandler = errorHandler;
            _messageProducer = messageProducer;

            hostApplicationLifetime.ApplicationStopped.Register(() =>
            {
                var currentProcess = Process.GetCurrentProcess();
                var sb = new StringBuilder();
                sb.AppendLine($"Total Processor Time: {currentProcess.TotalProcessorTime}");
                sb.AppendLine($"User Processor Time: {currentProcess.UserProcessorTime}");
                sb.AppendLine($"Process started: {currentProcess.StartTime.ToShortDateString()} {currentProcess.StartTime.ToLongTimeString()}");
                _logger.Info($"Application stopped. Killing {currentProcess.ProcessName}.", _correlationId, null, null, null, null);
                _logger.Info($"{currentProcess.ProcessName} has {DelayBeforeKillInSeconds} seconds to live.", _correlationId, null, null, null, null);
                _logger.Info(sb.ToString(), _correlationId, null, null, null, null);
                Thread.Sleep(DelayBeforeKillInSeconds * 1000);
                currentProcess.Kill();
            });

            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                _logger.Info("Application started.", _correlationId, null, null, null, null);
            });

            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                _logger.Info("Application stopping.", _correlationId, null, null, null, null);
            });
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _correlationId = Guid.NewGuid();
            _logger.Debug($"Entering {nameof(StartAsync)}.", _correlationId, null, null, null, null);

            if (Debugger.IsAttached)
            {
                _logger.Info($"{_serviceName}: debugger attached: NOT registering with service discovery.", _correlationId, null, null, null, null);
            }
            else
            {
                _serviceDiscoveryClient.Register();
                _logger.Info($"{_serviceName}: registered with service discovery.", _correlationId, null, null, null, null);
            }

            _logger.Info($"{_serviceName} has started.", _correlationId, null, null, null, null);
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _logger.Info($"{_serviceName} has been cancelled.", _correlationId, null, null, null, null));

            try
            {

                _logger.Debug($"Entering {nameof(ExecuteAsync)}.", _correlationId, null, null, null, null);

                _delay = _initialPollingIntervalInSeconds;

                while (!cancellationToken.IsCancellationRequested)
                {
                    _correlationId = Guid.NewGuid();

                    try
                    {
                        var resetPollingIntervalToDefault = await _messageProducer.Process(_correlationId);
                        _logger.Debug($"{_serviceName}: waiting {_delay} seconds.", _correlationId, null, null,
                            null, null);
                        SetPollingInterval(resetPollingIntervalToDefault);
                        _errorHandler.ResetKnownErrorsCount();
                    }
                    catch (Exception ex)
                    {
                        if (_errorHandler.Handle(ex, _correlationId))
                        {
                            continue;
                        }

                        _removeFromSd = false;
                        _logger.Fatal($"{_serviceName}: {ex.Message}", _correlationId, null, null, null, null, ex);
                        await StopAsync(cancellationToken);
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(_delay), cancellationToken);
                }
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        private void SetPollingInterval(bool resetPollingIntervalToDefault)
        {
            if (resetPollingIntervalToDefault)
            {
                _delay = _initialPollingIntervalInSeconds;
                return;
            }

            if (_delay < _maxPollingIntervalInSeconds)
            {
                _delay = Math.Min(_maxPollingIntervalInSeconds, _delay + _pollingIntervalIncrementInSeconds);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_removeFromSd)
            {
                _serviceDiscoveryClient.Remove();
                _logger.Info($"{_serviceName} removed from Service Discovery.", _correlationId, null, null, null, null);
            }

            return base.StopAsync(cancellationToken);
        }
    }
}
