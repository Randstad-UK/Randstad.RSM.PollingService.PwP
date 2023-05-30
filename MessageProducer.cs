using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.Services;
using Randstad.RSM.PollingService.PwP.Services.Api;
using Randstad.RSM.PollingService.PwP.Services.DataAccess;
using Randstad.RSM.PollingService.PwP.Settings;
using Randstad.RSM.PollingService.PwP.Template.Application;
using RSMServiceReference;
using System;
using System.Collections.Generic;
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

            var resp = _pwpService.CheckInvoicesHaveBeenPaid(correlationId);
       
            return false;
        }

    }
}
