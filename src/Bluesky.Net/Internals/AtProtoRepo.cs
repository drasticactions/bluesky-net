namespace Bluesky.Net.Internals;

using Commands.Bsky.Feed;
using Model;
using Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

internal class AtProtoRepo
{
    private readonly HttpClient _client;

    public AtProtoRepo(HttpClient client)
    {
        _client = client;
    }

    public Task<Result<CreatePostResponse>> Create(CreatePostRecord record, CancellationToken cancellationToken)
    {
        return
            _client
                .Post<CreatePostRecord, CreatePostResponse>(
                    Constants.Urls.AtProtoRepo.CreateRecord, record,
                    cancellationToken);
    }

    public Task<Result<CreatePostResponse>> Create(CreateLikeRecord record, CancellationToken cancellationToken)
    {
        return
            _client
                .Post<CreateLikeRecord, CreatePostResponse>(
                    Constants.Urls.AtProtoRepo.CreateRecord, record,
                    cancellationToken);
    }
}
