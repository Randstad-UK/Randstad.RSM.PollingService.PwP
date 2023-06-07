using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.Services;
using Randstad.RSM.PollingService.PwP.Template.Application;
using Randstad.RSM.PollingService.PwP.Template.Settings;
using System;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP
{
    internal class MessageProducer : IMessageProducer
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IPwpService _pwpService;
        private Guid _correlationId;

        public MessageProducer(ILogger logger, 
                               ApplicationSettings applicationSettings, 
                               IPwpService pwpService)
        {
            _logger = logger;
            _applicationSettings = applicationSettings;
            _pwpService = pwpService;
        }

        /// <summary>
        /// Produce a message if required and determine if the polling interval of the <see cref="MessageProducerService"/> should be reset.
        /// </summary>
        /// <param name="correlationId">The correlationId for the current iteration of the <see cref="MessageProducerService"/></param>
        /// <returns>True to set the polling interval to its initial value, false to increase the polling interval of the <see cref="MessageProducerService"/>.</returns>
        public async Task<bool> Process(Guid correlationId)
        {
            _logger.Debug($"Entering {nameof(Process)}.", correlationId, null, null, null, null);
            _correlationId = correlationId;

            try
            {
                await _pwpService.CheckInvoicesHaveBeenPaid(_applicationSettings.OpCo, correlationId);

                _logger.Debug($"Completed RSM invoice update, paid when paid ", correlationId, null, null, null, null);
            }
            catch (Exception ex)
            {
                _logger.Error($"Errors occured processing RSM invoices, paid when paid.", correlationId, null, null, null, null, ex);
                throw;
            }

            return true;
        }

    }
}
