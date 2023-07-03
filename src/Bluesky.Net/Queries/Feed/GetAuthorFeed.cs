namespace Bluesky.Net.Queries.Feed;

using Models;

public record GetAuthorFeed(AtUri Handle, int Limit, string? Cursor = default);
