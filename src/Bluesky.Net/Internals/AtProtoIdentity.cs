namespace Bluesky.Net.Internals;

using Models;
using Multiples;
using Queries.Feed;
using Queries.Model;
using System;
using System.Linq;
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
        string url = $"{Constants.Urls.Bluesky.GetAuthorFeed}?actor={query.Handle.Hostname}&limit={query.Limit}";
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

    internal async Task<Result<PostCollection>> GetPosts(GetPosts query, CancellationToken cancellationToken)
    {
        var answer = string.Join(",", query.Uris.Select(n => n.ToString()));
        string url = $"{Constants.Urls.Bluesky.GetPosts}?uris={answer}";
        Multiple<PostCollection?, Error> result = await _client.Get<PostCollection>(url, cancellationToken);
        return result
            .Match<Result<PostCollection>>(
                timeline => (timeline ?? new PostCollection(new PostView[0]))!,
                error => error!);
    }

    internal async Task<Result<LikesFeed>> GetLikes(GetLikes query, CancellationToken cancellationToken)
    {
        string url = $"{Constants.Urls.Bluesky.GetLikes}?uri={query.Uri.ToString()}&limit={query.Limit}";

        if (query.Cid is not null)
        {
            url += $"&cid={query.Cid}";
        }

        if (query.Cursor is not null)
        {
            url += $"&cursor={query.Cursor}";
        }
        Multiple<LikesFeed?, Error> result = await _client.Get<LikesFeed>(url, cancellationToken);
        return result
            .Match<Result<LikesFeed>>(
                timeline => (timeline ?? new LikesFeed(new Like[0], null))!,
                error => error!);
    }

    internal async Task<Result<RepostedFeed>> GetRepostedBy(GetRepostedBy query, CancellationToken cancellationToken)
    {
        string url = $"{Constants.Urls.Bluesky.GetRepostedBy}?uri={query.Uri.ToString()}&limit={query.Limit}";

        if (query.Cid is not null)
        {
            url += $"&cid={query.Cid}";
        }

        if (query.Cursor is not null)
        {
            url += $"&cursor={query.Cursor}";
        }
        Multiple<RepostedFeed?, Error> result = await _client.Get<RepostedFeed>(url, cancellationToken);
        return result
            .Match<Result<RepostedFeed>>(
                timeline => (timeline ?? new RepostedFeed(new Profile[0], null))!,
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
