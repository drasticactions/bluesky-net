namespace Bluesky.Net.Queries.Feed;

using Models;

public record GetTimeline(string Algorithm = "reverse-chronological", int Limit = 50, string? Cursor = default);
