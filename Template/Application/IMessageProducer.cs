using System;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Template.Application
{
    internal interface IMessageProducer
    {
        Task<bool> Process(Guid correlationId);
    }
}
