using ErrorOr;
using Forum.Application.Commands.Post.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Post;

public class GetPostRequest : IRequest<ErrorOr<PostResponse>>
{
    public Guid Id { get; set; }
}

public class GetPostRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<GetPostRequest, ErrorOr<PostResponse>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<PostResponse>> Handle(GetPostRequest request, CancellationToken cancellationToken)
    {
        var post = await _forumDbContext.Posts.SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if(post is null)
            return Error.NotFound(description: "post with given id not found");

        return post.Adapt<PostResponse>();
    }
}
