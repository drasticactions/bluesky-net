namespace Bluesky.Net.Commands.Bsky.Feed.Model;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// See <see cref="Link" /> and <see cref="Mention"/> 
/// </summary>
public class Facet
{
    /// <summary>
    /// The start and end of the the facet in the enriched text
    /// </summary>
    public ByteSlice Index { get; }
    
    /// <summary>
    /// The features of the Facet
    /// </summary>
    public List<FacetFeature> Features { get; }

    public Facet(ByteSlice index)
    {
        Index = index;
        this.Features = new List<FacetFeature>();
    }

    public void AddFeature(FacetFeature feature)
        => this.Features.Add(feature);
}
