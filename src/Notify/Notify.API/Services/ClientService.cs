using System.Text.Json;
using System.Text;
using Notify.API.Dtos;
using Polly;
using Notify.API.Interfaces;

namespace Notify.API.Services
{
    public class ClientService : IClientService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncPolicy _retryPolicy;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<ClientService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient.CreateClient();
            _retryPolicy =  Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<EmailBatchResponse?> GetUsersBatchAsync(Guid surveyId, int pageNumber, int pageSize, bool allusers)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var api = $"{_configuration["SurveyApi"]}participatnts";
                var apiKey = _configuration["API_KEY"];
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

                var requestData = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Id = surveyId,
                    AllUsers = allusers
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestData),
                    Encoding.UTF8,
                    "application/json"
                );
                var res = await _httpClient.PostAsync(api, content);
                if (!res.IsSuccessStatusCode)
                {
                    var c = await res.Content.ReadAsStringAsync();
                    _logger.LogError("HTTP Request Failed: {StatusCode} - {Content}", res.StatusCode, c);
                }
                //res.EnsureSuccessStatusCode();
                var result = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EmailBatchResponse>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            });
        }

    }
}
