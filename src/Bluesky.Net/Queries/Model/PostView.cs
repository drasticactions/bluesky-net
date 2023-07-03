namespace Bluesky.Net.Queries.Model;

using Bluesky.Net.Internals.Model;
using System;
using System.Collections.Generic;

public record PostView(
    string Uri,
    string Cid,
    int ReplyCount,
    int RepostCount,
    int LikeCount,
    Profile Author,
    PostRecord Record,
    DateTime IndexedAt,
    IReadOnlyList<Label> Labels
    );
