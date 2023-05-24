using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.Services.Import;
using Randstad.RSM.PollingService.PwP.Settings;
using Randstad.RSM.PollingService.PwP.Template.Application;
using RandstadMessageExchange;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP
{
    internal class MessageProducer : IMessageProducer
    {
        private readonly ILogger _logger;
        private readonly IProducerService _producerService;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IImportService _importService;

        public MessageProducer(ILogger logger, IProducerService producerService, ApplicationSettings applicationSettings, IImportService importService)
        {
            _logger = logger;
            _producerService = producerService;
            _applicationSettings = applicationSettings;
            _importService = importService;
        }

        /// <summary>
        /// Produce a message if required and determine if the polling interval of the <see cref="MessageProducerService"/> should be reset.
        /// </summary>
        /// <param name="correlationId">The correlationId for the current iteration of the <see cref="MessageProducerService"/></param>
        /// <returns>True to set the polling interval to its initial value, false to increase the polling interval of the <see cref="MessageProducerService"/>.</returns>
        public async Task<bool> Process(Guid correlationId)
        {
            _logger.Debug($"Entering {nameof(Process)}.", correlationId, null, null, null, null);

            var timesheetList = await _importService.ProcessAsync(correlationId);

            foreach (var timesheet in timesheetList)
            {
                var messageBody = System.Text.Json.JsonSerializer.Serialize(timesheet);
                PublishMessage(messageBody, _applicationSettings.RoutingKey, correlationId);

                _logger.Success(
                    $"{nameof(MessageProducer)}.{nameof(Process)}: Timesheet with timesheet-id {timesheet.Id}) published - routing key: {_applicationSettings.RoutingKey}",
                    timesheet.CorrelationId, timesheet, timesheet.ExternalTimesheetReference, null, null);
            }

            return false;
        }

        private void PublishMessage(string body, string routingKey, Guid correlationId)
        {
            var headers = new Dictionary<string, object>
            {
                { "CorrelationId", correlationId.ToString("D") }
            };
            _producerService.Publish(headers, correlationId, routingKey, body);
        }
    }
}
