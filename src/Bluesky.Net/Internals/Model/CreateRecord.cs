namespace Bluesky.Net.Internals.Model;

public record CreatePostRecord(string Collection, string Repo, PostRecord Record);

public record CreateLikeRecord(string Collection, string Repo, LikeRecord Record);
