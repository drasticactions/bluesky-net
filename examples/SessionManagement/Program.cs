using Bluesky.Net;
using Bluesky.Net.Commands.AtProto.Server;
using Bluesky.Net.Commands.Bsky.Feed;
using Bluesky.Net.Commands.Bsky.Feed.Model;
using Bluesky.Net.Json;
using Bluesky.Net.Models;
using Bluesky.Net.Queries.Feed;
using Bluesky.Net.Queries.Model;
using Microsoft.Extensions.DependencyInjection;
using Sharprompt;
using System.Text;
using System.Text.Json;


var testUri = new AtUri("tomwalker.bsky.social");
ServiceCollection services = new();
services.AddBluesky();
await using ServiceProvider sp = services.BuildServiceProvider();
JsonSerializerOptions printOptions =
    new()
    {
        WriteIndented = true,
        Converters =
        {
            new DidJsonConverter(), new AtUriJsonConverter(), new NsidJsonConverter(), new TidJsonConverter(), new FacetJsonConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

IBlueskyApi api = sp.GetRequiredService<IBlueskyApi>();

string userName = Environment.GetEnvironmentVariable("BLUESKY_USERNAME")!;
string password = Environment.GetEnvironmentVariable("BLUESKY_PASSWORD")!;
Login command = new(userName, password);
Result<Session> result = await api.Login(command, CancellationToken.None);
await result.SwitchAsync(async session =>
{
    Console.WriteLine("Logged in");
    Console.WriteLine(JsonSerializer.Serialize(session, printOptions));

    var exit = false;

    while(!exit)
    {
        var option = Prompt.Select<Menu>("Menu");

        switch (option)
        {
            case Menu.GetProfile:
                //Get the logged in user's profile
                Result<Profile> profileResult = await api.GetProfile(CancellationToken.None);

                profileResult.Switch(profile =>
                {
                    Console.WriteLine("User profile");
                    Console.WriteLine(JsonSerializer.Serialize(profile, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.ResolveHandle:
                //Resolve a user's DID
                Result<Did> resolvedHandle = await api.ResolveHandle(session.Handle, CancellationToken.None);

                resolvedHandle.Switch(handleResolved =>
                {
                    Console.WriteLine("Handle resolved");
                    Console.WriteLine(JsonSerializer.Serialize(handleResolved, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.RefreshToken:
                //Refresh the token
                Result<Session> result1 = await api.RefreshSession(session, CancellationToken.None);
                result1.Switch(refresh =>
                {
                    Console.WriteLine("Token refreshed");
                    Console.WriteLine(JsonSerializer.Serialize(refresh, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.GetAuthorFeed:
                var authorHandle = Prompt.Input<string>("Enter Author handle", defaultValue: "drasticactions.bsky.social");
                Result<AuthorFeed> authorFeed = await api.GetAuthorFeed(new GetAuthorFeed(new AtUri(authorHandle), 10), CancellationToken.None);
                authorFeed.Switch(refresh =>
                {
                    Console.WriteLine(JsonSerializer.Serialize(refresh, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.GetTimeline:
                Result<Timeline> timeline = await api.GetTimeline(new GetTimeline(Limit: 15), CancellationToken.None);
                timeline.Switch(refresh =>
                {
                    Console.WriteLine(JsonSerializer.Serialize(refresh, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.CreatePost:
                var prompt = Prompt.Input<string>("Enter post text", "Hello from API World");
                if (!string.IsNullOrEmpty(prompt))
                {
                    var facetList = new List<Facet>();
                    var addLink = Prompt.Input<bool>("Add link?");
                    
                    if (addLink)
                    {
                        var linkText = Prompt.Input<string>("Enter link text", "Link to Google");
                        prompt = $"{prompt} {linkText}";
                        int promptStart = prompt.IndexOf(linkText, StringComparison.InvariantCulture);
                        int promptEnd = promptStart + Encoding.Default.GetBytes(linkText).Length;
                        var linkUrl = Prompt.Input<string>("Enter link url", defaultValue: "https://www.google.com");
                        var facet = new Facet(new ByteSlice(promptStart, promptEnd));
                        facet.AddFeature(new Link(linkUrl));
                        facetList.Add(facet);
                    }

                    var test = JsonSerializer.Serialize(facetList.ToArray(), printOptions);
                    CreatePost post = new(
                        prompt, facetList.ToArray());
                    Result<CreatePostResponse> created = await api.CreatePost(post, CancellationToken.None);
                    created.Switch(x =>
                    {
                        Console.WriteLine(JsonSerializer.Serialize(x, printOptions));
                    }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                }
                break;
            case Menu.GetLikes:
                var likes = await api.GetLikes(new GetLikes(new AtUri("at://did:plc:juem56avlegp5z4hctyxzg7z/app.bsky.feed.post/3jzlxh3fay52u")), CancellationToken.None);
                likes.Switch(refresh =>
                {
                    Console.WriteLine(JsonSerializer.Serialize(refresh, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.GetRepostedBy:
                var reposted = await api.GetRepostedBy(new GetRepostedBy(new AtUri("at://did:plc:juem56avlegp5z4hctyxzg7z/app.bsky.feed.post/3jzlxh3fay52u")), CancellationToken.None);
                reposted.Switch(refresh =>
                {
                    Console.WriteLine(JsonSerializer.Serialize(refresh, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.GetPosts:
                var posts = await api.GetPosts(new GetPosts(new List<AtUri>() { new AtUri("at://did:plc:juem56avlegp5z4hctyxzg7z/app.bsky.feed.post/3jzlxh3fay52u") }), CancellationToken.None);
                posts.Switch(refresh =>
                {
                    Console.WriteLine(JsonSerializer.Serialize(refresh, printOptions));
                }, _ => Console.WriteLine(JsonSerializer.Serialize(_, printOptions)));
                break;
            case Menu.Exit:
            default:
                exit = true;
                break;
        }
    }
}, _ =>
{
    Console.WriteLine(JsonSerializer.Serialize(_, printOptions));
    return Task.CompletedTask;
});

enum Menu
{
    GetProfile,
    ResolveHandle,
    RefreshToken,
    GetLikes,
    GetRepostedBy,
    GetPosts,
    GetAuthorFeed,
    GetTimeline,
    CreatePost,
    Exit
}
