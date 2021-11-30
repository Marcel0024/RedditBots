using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditBots.Libraries.Logging;

public class HttpLoggerService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly HttpLoggerOptions _options;

    private readonly Uri _baseUri;

    public HttpLoggerService(IHttpClientFactory clientFactory, IOptions<HttpLoggerOptions> options)
    {
        _clientFactory = clientFactory;
        _options = options.Value;
        _baseUri = new Uri(_options.Uri);
    }

    public async Task PostLogAsync(string json, CancellationToken cancellationToken)
    {
        var client = _clientFactory.CreateClient(nameof(HttpLoggerService));

        client.BaseAddress = _baseUri;
        client.Timeout = TimeSpan.FromSeconds(1);

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-APIKEY", _options.ApiKey);
        }

        using StringContent content = new(json, Encoding.UTF8, "application/json");

        (await client.PostAsync("", content, cancellationToken))
            .EnsureSuccessStatusCode();
    }
}
