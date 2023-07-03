namespace Bluesky.Net.Internals;

using Models;
using Multiples;
using Queries.Feed;
using Queries.Model;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

internal class BlueskyFeed
{
    private readonly HttpClient _client;

    public BlueskyFeed(HttpClient client)
    {
        _client = client;
    }

    internal async Task<Result<AuthorFeed>> GetAuthorFeed(GetAuthorFeed query, CancellationToken cancellationToken)
    {
        string url = $"{Constants.Urls.Bluesky.GetAuthorFeed}?actor={query.Handle}&limit={query.Limit}";
        if (query.Cursor is not null)
        {
            url += $"&cursor={query.Cursor}";
        }

        Multiple<AuthorFeed?, Error> result = await _client.Get<AuthorFeed>(url, cancellationToken);
        return result
            .Match<Result<AuthorFeed>>(
                authorFeed => (authorFeed ?? new AuthorFeed(Array.Empty<FeedViewPost>(), null))!,
                error => error!);
    }

    internal async Task<Result<Timeline>> GetTimeline(GetTimeline query, CancellationToken cancellationToken)
    {
        string url = $"{Constants.Urls.Bluesky.GetTimeline}?algorithm={query.Algorithm}&limit={query.Limit}";
        if (query.Cursor is not null)
        {
            url += $"&cursor={query.Cursor}";
        }

        Multiple<Timeline?, Error> result = await _client.Get<Timeline>(url, cancellationToken);
        return result
            .Match<Result<Timeline>>(
                timeline => (timeline ?? new Timeline(Array.Empty<FeedViewPost>(), null))!,
                error => error!);
    }
}

internal class AtProtoIdentity
{
    private readonly HttpClient _client;

    internal AtProtoIdentity(HttpClient client)
    {
        _client = client;
    }

    internal async Task<Result<Did>> ResolveHandle(string handle, CancellationToken cancellationToken)
    {
        string url = $"{Constants.Urls.AtProtoIdentity.ResolveHandle}?handle={handle}";
        Result<HandleResolution?> result = await _client.Get<HandleResolution>(url, cancellationToken);
        return result.Match(resolution =>
        {
            Result<Did> did = resolution!.Did!;
            return did;
        }, error => error!);
    }
}
