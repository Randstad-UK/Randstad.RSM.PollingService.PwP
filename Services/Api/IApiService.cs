using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.Api
{
    public interface IApiService
    {
        Task ProcessPostRequestAsync(string apiRequestUrl, object content, Guid correlationId, Dictionary<string, string> customHttpHeaders = null);
        Task<T> ProcessGetRequestAsync<T>(string apiRequestUrl, Guid correlationId, Dictionary<string, string> customHttpHeaders = null);
    }
}
