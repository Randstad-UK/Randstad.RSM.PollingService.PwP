﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Randstad.Logging;
using RSMServiceReference;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Randstad.RSM.PollingService.PwP.Settings;

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

        public async Task<List<InvoiceInfo>> GetUnpaidInvoiceNumbers(Guid correlationId)
        {
            var url = _rsmApiSettings.GetInvoicesNotPaidEndpoint;

            _logger.Debug($"Get invoicea not paid: URL {url}", correlationId, null, null, null, null);

            var resp = await _apiService.ProcessPostRequestAsync<string>(url, null, correlationId);

            var @return = JObject.Parse(resp).SelectToken("return");

            List<InvoiceInfo> invoices = JsonConvert.DeserializeObject<List<InvoiceInfo>>(@return.ToString());

            return invoices;
        }          

        public async Task<bool> UpdateInvoiceToBePaid(string invoiceNumber, Guid correlationId)
        {
            var url = _rsmApiSettings.UpdateInvoiceToBePaidEndpoint.Replace("{invoiceNumber}", invoiceNumber);

            _logger.Debug($"Set invoice to be paid: URL {url}", correlationId, null, invoiceNumber, "Invoice Number", null);

            HttpResponseMessage response = await _apiService.ProcessPatchRequestAsync(url, correlationId);

            _logger.Info($"Set invoice to be paid returned response;", correlationId, response, response.StatusCode.ToString(), "Invoice Number", null);

            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

    }
}