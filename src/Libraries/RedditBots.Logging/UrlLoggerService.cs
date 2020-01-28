using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Logging
{
    public class UrlLoggerService
    {
        private readonly HttpClient _client;
        private readonly UrlLoggerOptions _options;

        public UrlLoggerService(HttpClient client, IOptions<UrlLoggerOptions> options)
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
