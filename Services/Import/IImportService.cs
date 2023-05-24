using Randstad.RSM.PollingService.PwP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.Import
{
    public interface IImportService
    {
        Task<IEnumerable<Timesheet>> ProcessAsync(Guid correlationId);
    }
}
