namespace Bluesky.Net.Json;

using Bluesky.Net.Commands.Bsky.Feed.Model;
using Bluesky.Net.Internals;
using Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FacetJsonConverter : JsonConverter<Facet?>
{
    public override bool CanConvert(Type type)
    {
        return type.IsAssignableFrom(typeof(Facet));
    }

    public override Facet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            if (doc.RootElement.TryGetProperty("index", out var index))
            {
                var byteSlice = JsonSerializer.Deserialize<ByteSlice>(index.GetRawText(), options);
                if (byteSlice is null)
                {
                    return null;
                }
                
                var facet = new Facet(byteSlice);

                if (doc.RootElement.TryGetProperty("features", out var type))
                {
                    foreach (JsonElement element in type.EnumerateArray())
                    {
                        if (element.TryGetProperty("$type", out var featureType))
                        {
                            switch(featureType.GetString())
                            {
                                case Constants.FacetTypes.Mention:
                                    facet.Features.Add(JsonSerializer.Deserialize<Mention>(element.GetRawText(), options)!);
                                    break;
                                case Constants.FacetTypes.Link:
                                    facet.Features.Add(JsonSerializer.Deserialize<Link>(element.GetRawText(), options)!);
                                    break;
                                default:
#if DEBUG
                                    throw new NotImplementedException();
#else
                                    break;
#endif
                            }

                            // TODO: Add more types.
                        }
                    }
                }

                return facet;
            }

            return null;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, Facet? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value);
    }
}
