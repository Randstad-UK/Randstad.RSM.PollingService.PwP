using System;

namespace Randstad.RSM.PollingService.PwP.Template.Application
{
    internal interface IErrorHandler
    {
        void ResetKnownErrorsCount();
        bool Handle(Exception ex, Guid correlationId);
    }
}