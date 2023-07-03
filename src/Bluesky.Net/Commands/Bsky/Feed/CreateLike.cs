namespace Bluesky.Net.Commands.Bsky.Feed;

using Bluesky.Net.Models;
using Model;
using System;

public record CreateLike(
    string Cid,
    AtUri Uri,
    DateTime? CreatedAt = default
    )
{
}
