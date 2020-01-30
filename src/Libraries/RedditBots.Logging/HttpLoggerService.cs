using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Libraries.Logging
{
    public class HttpLoggerService
    {
        private readonly HttpClient _client;
        private readonly HttpLoggerOptions _options;

        public HttpLoggerService(HttpClient client, IOptions<HttpLoggerOptions> options)
        {
            _client = client;
            _options = options.Value;

            if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _client.DefaultRequestHeaders.Add("X-APIKEY", _options.ApiKey);
            }
        }

        public async Task PostLogAsync(string json, CancellationToken cancellationToken)
        {
            using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            (await _client.PostAsync(_options.Url, content, cancellationToken))
                .EnsureSuccessStatusCode();
        }
    }
}
