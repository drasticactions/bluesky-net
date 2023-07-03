namespace Bluesky.Net.Internals;

using Bluesky.Net.Commands.Bsky.Feed.Model;
using Json;
using Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions? Options = new()
    {
        Converters =
        {
            new DidJsonConverter(),
            new AtUriJsonConverter(),
            new NsidJsonConverter(),
            new TidJsonConverter(),
            new FacetJsonConverter()
        }, PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    internal async static Task<Result<TK>> Post<T, TK>(
        this HttpClient client,
        string url,
        T body,
        CancellationToken cancellationToken)
    {
        var jsonContent = JsonSerializer.Serialize(body, Options);
        StringContent content = new(jsonContent, Encoding.UTF8, "application/json");
        #if DEBUG
        System.Diagnostics.Debug.WriteLine($"POST {url}: {jsonContent}");
        #endif
        var message = await client.PostAsync(url, content, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            Error error = await CreateError(message!, cancellationToken);
            return error!;
        }

        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        TK? result = JsonSerializer.Deserialize<TK>(response, Options);
        return result!;
    }

    internal async static Task<Result<TK>> Post<TK>(
        this HttpClient client,
        string url,
        CancellationToken cancellationToken)
    {
        var message = await client.PostAsync(url, null, cancellationToken: cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            Error error = await CreateError(message!, cancellationToken);
            return error!;
        }

        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        TK? result = JsonSerializer.Deserialize<TK>(response, Options);
        return result!;
    }

    internal static async Task<Result<T?>> Get<T>(
        this HttpClient client,
        string url,
        CancellationToken cancellationToken)
    {
        var message = await client.GetAsync(url, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            Error error = await CreateError(message!, cancellationToken);
            return error!;
        }

        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(response, Options);
    }

    private static async Task<Error> CreateError(HttpResponseMessage message, CancellationToken cancellationToken)
    {
        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        ErrorDetail? detail = default;
        if (string.IsNullOrEmpty(response))
        {
            return new Error((int)message.StatusCode, detail);
        }

        try
        {
            detail = JsonSerializer.Deserialize<ErrorDetail>(response, Options);
        }
        catch (Exception)
        {
            // ignored
        }

        return new Error((int)message.StatusCode, detail);
    }
}

