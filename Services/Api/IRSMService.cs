using RSMServiceReference;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Randstad.RSM.PollingService.PwP.Services.Api
{
    public interface IRSMService
    {
        Task<List<string>> GetUnpaidInvoiceRefCodes(string opCo, Guid correlationId);
        Task<List<InvoiceInfo>> GetInvoicesByRefCodes(List<string> invoiceRefs, string opCo, Guid correlationId);
        Task<bool> UpdateInvoiceToBePaid(string url, Guid correlationId);
    }
}
