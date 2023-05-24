using Randstad.Logging;
using Randstad.RSM.PollingService.PwP.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Randstad.RSM.PollingService.PwP.Services.Api
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApiService(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task ProcessPostRequestAsync(string apiRequestUrl, object content, Guid correlationId,
            Dictionary<string, string> customHttpHeaders = null)
        {
            await DoCallApiAsync(HttpMethod.Post, apiRequestUrl, customHttpHeaders, correlationId, content);
        }

        public async Task<T> ProcessGetRequestAsync<T>(string apiRequestUrl, Guid correlationId, Dictionary<string, string> customHttpHeaders = null)
        {
            var response = await DoCallApiAsync(HttpMethod.Get, apiRequestUrl, customHttpHeaders, correlationId);
            return JsonSerializer.Deserialize<T>(response, _jsonSerializerOptions);
        }

        private async Task<string> DoCallApiAsync(HttpMethod httpMethod, string apiRequestUrl, Dictionary<string, string> customHttpHeaders, Guid correlationId, object content = null)
        {
            try
            {
                var request = new HttpRequestMessage(httpMethod, apiRequestUrl);
                request.Headers.Accept.Clear();

                if (customHttpHeaders != null)
                {
                    foreach (var (key, value) in customHttpHeaders)
                    {
                        request.Headers.Add(key, value);
                    }
                }

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add(Constants.CorrelationIdApiKey, correlationId.ToString());

                if (content != null)
                {
                    var jsonContent = JsonSerializer.Serialize(content);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                return data;
            }
            catch (Exception e)
            {
                _logger.Error(
                    $"{nameof(ApiService)}.{nameof(DoCallApiAsync)} - endpoint: {_httpClient.BaseAddress}{apiRequestUrl} message: {e.Message}",
                    correlationId, content, null, null, null, e);
                throw;
            }
        }

    }


}