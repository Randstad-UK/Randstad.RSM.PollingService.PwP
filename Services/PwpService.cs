using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.Models;
using Randstad.RSM.PollingService.PwP.Services.Api;
using Randstad.RSM.PollingService.PwP.Services.DataAccess;
using Randstad.RSM.PollingService.PwP.Settings;
using Randstad.RSM.PollingService.PwP.Template.Settings;
using RSMServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services
{
    internal class PwpService : IPwpService
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly RsmApiSettings _rsmApiSettings;
        private readonly IDreamDAL _dreamDAL;
        private readonly IRSMService _rsmService;
        private readonly ILogger _logger;

        public PwpService(IDreamDAL dreamDAL,
                          IRSMService rsmService,
                          ApplicationSettings applicationSettings,
                          RsmApiSettings rsmApiSettings,
                          ILogger logger)
        {
            _logger = logger;
            _dreamDAL = dreamDAL;
            _rsmService = rsmService;
            _rsmApiSettings = rsmApiSettings;
            _applicationSettings = applicationSettings;
        }

        public async Task CheckInvoicesHaveBeenPaid(Guid correlationId)
        {
            List<InvoiceInfo> invoices = await _rsmService.GetUnpaidInvoiceNumbers(correlationId);
            string[] paidStatuses = _applicationSettings.PaidStatuses.Split(',');

            foreach (InvoiceInfo inv in invoices)
            {
                DreamInvoice invoice =_dreamDAL.GetInvoiceByInvoiceNumber(inv.invoiceNumber, correlationId).Result;

                if (invoice != null && paidStatuses.Contains(invoice.DetailStatus))
                {
                    await _rsmService.UpdateInvoiceToBePaid(inv.invoiceNumber, correlationId);
                }
            }
        }
    }
}
