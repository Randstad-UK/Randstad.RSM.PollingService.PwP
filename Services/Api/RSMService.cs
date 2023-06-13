using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Randstad.Logging;
using RSMServiceReference;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Randstad.RSM.PollingService.PwP.Settings;
using Randstad.RSM.PollingService.PwP.Template.Settings;

namespace Randstad.RSM.PollingService.PwP.Services.Api
{
    internal class RSMService : IRSMService
    {
        private readonly RsmApiSettings _rsmApiSettings;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IApiService _apiService;
        private readonly ILogger _logger;

        public RSMService(IApiService apiService, RsmApiSettings rsmApiSettings, ApplicationSettings applicationSettings, ILogger logger)
        {
            _logger = logger;
            _apiService = apiService;
            _rsmApiSettings = rsmApiSettings;
            _applicationSettings = applicationSettings;
        }

        public async Task<List<string>> GetUnpaidInvoiceRefCodes(string opCo, Guid correlationId)
        {
            var url = _rsmApiSettings.GetUnpaidInvoiceRefCodesEndpoint.Replace("{opCo}", opCo); 

            _logger.Debug($"Get invoices references not paid: URL {url}", correlationId, null, null, null, null);

            var resp = await _apiService.ProcessGetRequestAsync<string>(url, correlationId);

            var @return = JObject.Parse(resp).SelectToken("return");

            List<string> invoices = JsonConvert.DeserializeObject<List<string>>(@return.ToString());

            return invoices;
        }

        public async Task<List<InvoiceInfo>> GetInvoicesByRefCodes(List<string> invoiceRefs, string opCo, Guid correlationId)
        {
            var url = _rsmApiSettings.GetInvoicesByRefCodesEndpoint.Replace("{opCo}", opCo);

            _logger.Debug($"Get invoices that are not paid: URL {url}", correlationId, null, null, null, null);

            var resp = await _apiService.ProcessPostRequestAsync<string>(url, invoiceRefs, correlationId);

            var @return = JObject.Parse(resp).SelectToken("return");

            List<InvoiceInfo> invoices = JsonConvert.DeserializeObject<List<InvoiceInfo>>(@return.ToString());
              
            return invoices;
        }

        public async Task<bool> UpdateInvoiceToBePaid(string invoiceGuid, string opCo, Guid correlationId)
        {
            var url = _rsmApiSettings.UpdateInvoiceAsPaidEndpoint.Replace("{invoiceGuid}", invoiceGuid).Replace("{opCo}", opCo);

            _logger.Debug($"Set invoice to be paid: URL {url}", correlationId, null, invoiceGuid, "Invoice Guid", null);

            HttpResponseMessage response = await _apiService.ProcessPatchRequestAsync(url, correlationId);

            _logger.Info($"Set invoice to be paid returned response;", correlationId, response, response.StatusCode.ToString(), "Invoice Guid", null);

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

    }
}