namespace EfersonTesting.EfersonSilva.Interfaces
{
    public interface IHttpClientRequestLog
    {
        public Task<string> HttpClientGetAsync(string path, string parameter);
    }
}
