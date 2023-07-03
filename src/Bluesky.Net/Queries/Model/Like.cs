namespace Bluesky.Net.Queries.Model;

using System;

public record Like(Profile Actor, DateTime CreatedAt, DateTime IndexedAt);
