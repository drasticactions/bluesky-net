namespace Bluesky.Net.Internals;

using Commands.AtProto.Server;
using Commands.Bsky.Feed;
using Model;
using Models;
using Queries.Feed;
using Queries.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

internal class BlueskyApi : IBlueskyApi, IDisposable
{
    private readonly BlueskyApiOptions _options;
    private readonly AtProtoServer _server;
    private readonly HttpClient _client;
    private readonly AtProtoIdentity _identity;
    private readonly BlueskyFeed _feed;
    private readonly BlueskyActor _actor;
    private readonly AtProtoRepo _repo;

    private SessionManager? _sessionManager;

    public BlueskyApi(IHttpClientFactory factory, BlueskyApiOptions options)
    {
        _options = options;
        _client = factory.CreateClient(Constants.BlueskyApiClient);
        _server = new(factory, _options);
        _identity = new(_client);
        _repo = new(_client);
        _actor = new(_client);
        _feed = new(_client);
        _server.UserLoggedIn += OnUserLoggedIn;
        _server.TokenRefreshed += UpdateBearerToken;
    }

    public Task<Result<Session>> Login(Login command, CancellationToken cancellationToken)
    {
        return _server.Login(command, cancellationToken);
    }

    public Task<Result<Session>> RefreshSession(
        Session session,
        CancellationToken cancellationToken) => _server.RefreshSession(session, cancellationToken);

    public Task<Result<Profile>> GetProfile(Did did, CancellationToken cancellationToken)
        => _actor.GetProfile(did, cancellationToken)!;

    public Task<Result<Profile>> GetProfile(CancellationToken cancellationToken)
        => GetProfile(_sessionManager.ThrowIfNull().Session.ThrowIfNull().Did, cancellationToken);

    public Task<Result<Did>> ResolveHandle(
        string handle,
        CancellationToken cancellationToken) => _identity.ResolveHandle(handle, cancellationToken);

    public Task<Result<CreatePostResponse>> CreatePost(
        CreatePost command,
        CancellationToken cancellationToken)
    {
        CreatePostRecord record = new(
            Constants.FeedType.Post,
            _sessionManager!.Session!.Did.ToString()!,
            new PostRecord()
            {
                Text = command.Text,
                Type = Constants.FeedType.Post,
                CreatedAt = command.CreatedAt ?? DateTime.UtcNow,
                Facets = command.Facets
            });

        return _repo.Create(record, cancellationToken);
    }

    public Task<Result<CreatePostResponse>> CreateLike(
    CreateLike command,
    CancellationToken cancellationToken)
    {
        CreateLikeRecord record = new(
            Constants.FeedType.Like,
            _sessionManager!.Session!.Did.ToString()!,
            new LikeRecord()
            {
                Subject = new Subject() {
                    Cid = command.Cid,
                    Uri = command.Uri.ToString(),
                },
                Type = Constants.FeedType.Like,
                CreatedAt = command.CreatedAt ?? DateTime.UtcNow,
            });

        return _repo.Create(record, cancellationToken);
    }

    public Task<Result<AuthorFeed>> GetAuthorFeed(GetAuthorFeed query, CancellationToken cancellationToken)
        => _feed.GetAuthorFeed(query, cancellationToken);

    public Task<Result<Timeline>> GetTimeline(GetTimeline query, CancellationToken cancellationToken)
        => _feed.GetTimeline(query, cancellationToken);

    public Task<Result<PostCollection>> GetPosts(GetPosts query, CancellationToken cancellationToken)
        => _feed.GetPosts(query, cancellationToken);

    public Task<Result<LikesFeed>> GetLikes(GetLikes query, CancellationToken cancellationToken)
        => _feed.GetLikes(query, cancellationToken);

    public Task<Result<RepostedFeed>> GetRepostedBy(GetRepostedBy query, CancellationToken cancellationToken)
        => _feed.GetRepostedBy(query, cancellationToken);

    public void Dispose()
    {
        _sessionManager?.Dispose();
    }

    private void UpdateBearerToken(Session session)
    {
        _client
                .DefaultRequestHeaders
                .Authorization =
            new AuthenticationHeaderValue("Bearer", session.AccessJwt);
    }

    private void OnUserLoggedIn(Session session)
    {
        UpdateBearerToken(session);

        if (!_options.TrackSession)
        {
            return;
        }

        if (_sessionManager is null)
        {
            _sessionManager = new SessionManager(_options, _server, session);
        }
        else
        {
            _sessionManager.SetSession(session);
        }
    }
}
