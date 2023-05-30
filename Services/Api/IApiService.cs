using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.Api
{
    public interface IApiService
    {
        Task<T> ProcessPostRequestAsync<T>(string apiRequestUrl, object content, Guid correlationId, Dictionary<string, string> customHttpHeaders = null);
        Task<HttpResponseMessage> ProcessPostRequestAsync(string apiRequestUrl, object content, Guid correlationId, Dictionary<string, string> customHttpHeaders = null);
        Task<T> ProcessGetRequestAsync<T>(string apiRequestUrl, Guid correlationId, Dictionary<string, string> customHttpHeaders = null);
        Task<HttpResponseMessage> ProcessPatchRequestAsync(string url, Guid correlationId, Dictionary<string, string> customHttpHeaders = null);
    }
}
