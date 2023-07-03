namespace Bluesky.Net.Commands.Bsky.Feed.Model;

using System.Text.Json.Serialization;

public abstract class FacetFeature
{
    [JsonPropertyName("$type")] public abstract string Type { get; }
}
