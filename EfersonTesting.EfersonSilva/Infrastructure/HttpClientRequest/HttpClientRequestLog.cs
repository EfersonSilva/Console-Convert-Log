using EfersonTesting.EfersonSilva.Interfaces;

namespace EfersonTesting.EfersonSilva.Infrastructure.HttpClientRequest
{
    public class HttpClientRequestLog: IHttpClientRequestLog
    {
        private readonly ILogger<HttpClientRequestLog> _logger;

        public HttpClientRequestLog(ILogger<HttpClientRequestLog> logger)
        {
            _logger = logger;
        }

        public async Task<string> HttpClientGetAsync(string path, string parameter)
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(path) };
            var responseAsync = await client.GetAsync(parameter);
            string content = await responseAsync.Content.ReadAsStringAsync();

            _logger.LogInformation("Request StatusCode: {responseAsync.StatusCode}, Response: {content}", responseAsync.StatusCode, content);

            return content;
        }
    }
}
