using RSMServiceReference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Randstad.RSM.PollingService.PwP.Services.Api
{
    public interface IRSMService
    {
        Task<List<InvoiceInfo>> GetUnpaidInvoiceNumbers(Guid correlationId);

        Task<bool> UpdateInvoiceToBePaid(string url, Guid correlationId);
    }
}
