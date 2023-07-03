namespace Bluesky.Net.Queries.Feed;

using Models;
using System.Collections.Generic;

public record GetPosts(IEnumerable<AtUri> Uris);
