namespace Bluesky.Net.Commands.Bsky.Feed.Model;

using Bluesky.Net.Internals;

/// <summary>
/// A Link inside a enriched text
/// </summary>
public class Link : FacetFeature
{
    /// <summary>
    /// Ctor 
    /// </summary>
    /// <param name="uri">A Uri <see cref="Uri"/></param>
    public Link(string uri)
    {
        Uri = uri.StartsWith("http") ? uri : $"https://{uri}";
    }

    /// <summary>
    /// The resolved Uri of the text
    /// If the value doesnt start with http it will have appended https
    /// </summary>
    public string Uri { get; }

    public override string Type => Constants.FacetTypes.Link;
}
