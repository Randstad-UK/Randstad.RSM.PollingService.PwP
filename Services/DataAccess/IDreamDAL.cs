using Randstad.RSM.PollingService.PwP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.DataAccess
{
    public interface IDreamDAL
    {
        Task<DreamInvoice> GetInvoiceByInvoiceNumber(string invoiceNumber, Guid correlationId);

    }
}
