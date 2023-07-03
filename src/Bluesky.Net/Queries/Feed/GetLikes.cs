namespace Bluesky.Net.Queries.Feed;

using Models;
using System.Collections.Generic;

public record GetLikes(AtUri Uri, int Limit = 50, string? Cid = default, string? Cursor = default);

public record GetRepostedBy(AtUri Uri, int Limit = 50, string? Cid = default, string? Cursor = default);
