namespace Bluesky.Net.Internals.Model;

using Bluesky.Net.Models;
using Commands.Bsky.Feed.Model;
using System;
using System.Text.Json.Serialization;

public class PostRecord : Record
{
    public string Text { get; set; }

    public Facet[]? Facets { get; set; } = Array.Empty<Facet>();

    public string[] Langs { get; set; } = Array.Empty<string>();

    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("$type")] public string Type { get; set; }
}

public class LikeRecord 
{
    public Subject Subject { get; set; }

    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("$type")] public string Type { get; set; }
}

public class Subject
{
    public string Cid { get; set; }

    public string Uri { get; set; }
}

public class Record
{

    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("$type")] public string Type { get; set; }
}

