namespace Bluesky.Net.Commands.Bsky.Feed;

using Model;
using System;

/// <summary>
/// A command to create a post
/// </summary>
/// <param name="Text">The text</param>
/// <param name="Facets">The <see cref="Facet"/> of the text</param>
public record CreatePost(
    string Text,
    Facet[]? Facets,
    DateTime? CreatedAt = default
    )
{
}
