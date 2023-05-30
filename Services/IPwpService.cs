using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services
{
    internal interface IPwpService
    {
        Task CheckInvoicesHaveBeenPaid(Guid correlationId);
    }
}
