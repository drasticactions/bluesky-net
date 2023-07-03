namespace Bluesky.Net.Json;

using Bluesky.Net.Commands.Bsky.Feed.Model;
using Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ObjectArrayConverter<T> : JsonConverter<T[]>
{
    public override T[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var list = new List<T>();

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var item = JsonSerializer.Deserialize<T>(ref reader, options);
            list.Add(item);
        }

        return list.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, T[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var item in value)
        {
            JsonSerializer.Serialize(writer, item, options);
        }

        writer.WriteEndArray();
    }
}
