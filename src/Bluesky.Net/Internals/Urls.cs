namespace Bluesky.Net.Internals;

internal static class Constants
{
    internal const string BlueskyApiClient = "Bluesky";
    internal const string ContentMediaType = "application/json";
    internal const string AcceptedMediaType = "application/json";

    internal static class Urls
    {
        internal static class AtProtoServer
        {
            internal const string Login = "/xrpc/com.atproto.server.createSession";
            internal const string RefreshSession = "/xrpc/com.atproto.server.refreshSession";
        }

        internal static class AtProtoIdentity
        {
            internal const string ResolveHandle = "/xrpc/com.atproto.identity.resolveHandle";
        }

        internal static class AtProtoRepo
        {
            internal const string CreateRecord = "/xrpc/com.atproto.repo.createRecord";
        }

        internal static class Bluesky
        {
            internal const string GetAuthorFeed = "/xrpc/app.bsky.feed.getAuthorFeed";
            internal const string GetTimeline = "/xrpc/app.bsky.feed.getTimeline";
            internal const string GetPosts = "/xrpc/app.bsky.feed.getPosts";
            internal const string GetLikes = "/xrpc/app.bsky.feed.getLikes";
            internal const string GetRepostedBy = "/xrpc/app.bsky.feed.getRepostedBy";
            internal const string GetActorProfile = "/xrpc/app.bsky.actor.getProfile";
        }

    }

    internal class HeaderNames
    {
        internal const string UserAgent = "user-agent";
    }

    internal static class FacetTypes
    {
        internal const string Link = "app.bsky.richtext.facet#link";
        internal const string Mention = "app.bsky.richtext.facet#mention";
    }
}
