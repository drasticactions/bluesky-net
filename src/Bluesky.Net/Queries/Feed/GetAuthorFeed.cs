namespace Bluesky.Net.Queries.Feed;

using Models;

public record GetAuthorFeed(string Handle, int Limit, string? Cursor = default);
