using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots.Logging
{
    public class RedditBotsLoggerService
    {
        private readonly HttpClient _client;
        private readonly RedditBotsLoggerOptions _options;

        public RedditBotsLoggerService(HttpClient client, IOptions<RedditBotsLoggerOptions> options)
        {
            _client = client;
            _options = options.Value;

            _client.DefaultRequestHeaders.Add("ApiKey", _options.ApiKey);
        }

        public Task PostLogAsync(string json)
        {
            using StringContent content = new StringContent(json, Encoding.UTF8, "appliction/json");
            return _client.PostAsync(_options.Url, content);
        }
    }
}
