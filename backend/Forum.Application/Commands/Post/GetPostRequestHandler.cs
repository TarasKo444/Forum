using Forum.Common;
using Forum.Domain;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application;

public class GetPostRequest : IRequest<PostResponse>
{
    public Guid Id { get; set; }
}

public class GetPostRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<GetPostRequest, PostResponse>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<PostResponse> Handle(GetPostRequest request, CancellationToken cancellationToken)
    {
        var post = await _forumDbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        Throw.ApiExceptionIfNull(post, 400, new("post with provided [{0}] not found", nameof(request.Id)));

        return post.Adapt<PostResponse>();
    }
}
