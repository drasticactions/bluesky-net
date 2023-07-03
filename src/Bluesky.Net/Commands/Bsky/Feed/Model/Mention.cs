namespace Bluesky.Net.Commands.Bsky.Feed.Model;

using Bluesky.Net.Internals;
using Models;

/// <summary>
/// A mention to an actor
/// </summary>
public class Mention : FacetFeature
{
    /// <summary>
    /// Ctor 
    /// </summary>
    /// <param name="did">A <see cref="Did"/> of the actor</param>
    public Mention(Did did)
    {
        Did = did;
    }

    /// <summary>
    /// A <see cref="Did"/> of the actor
    /// </summary>
    public Did Did { get; }

    public override string Type => Constants.FacetTypes.Mention;
}
