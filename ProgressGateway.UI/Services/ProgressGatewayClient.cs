using System.Net.Http.Json;

namespace ProgressGateway.UI.Services
{
    public class ProgressGatewayClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProgressGatewayClient> _logger;

        public ProgressGatewayClient(
            HttpClient httpClient,
            ILogger<ProgressGatewayClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task UpdateProgressAsync(
            string executionId,
            string step,
            string status,
            int percentage,
            string message)
        {
            var request = new
            {
                ExecutionId = executionId,
                Step = step,
                Status = status,
                Percentage = percentage,
                Message = message
            };

            _logger.LogInformation(
                "Sending progress update: {@Request}",
                request);


            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "api/Progress/update",
                    request);


            // Read the response body BEFORE throwing exception
            string responseContent =
                await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Progress API failed. Status: {StatusCode}. Response: {Response}",
                    response.StatusCode,
                    responseContent);


                throw new Exception(
                    $"Progress API Error. " +
                    $"Status Code: {(int)response.StatusCode}. " +
                    $"Response: {responseContent}"
                );
            }

            _logger.LogInformation(
                "Progress update successful."
            );
        }
    }
}