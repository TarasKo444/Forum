using ErrorOr;
using Forum.Application.Commands.Post.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Post;

public class EditPostRequest : IRequest<ErrorOr<PostResponse>>
{
    public Guid Id { get; set; }
    public string Header { get; set; } = null!;
    public string Body { get; set; } = null!;
    public Guid PostCreatorId { get; set; }
}

public class EditPostRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<EditPostRequest, ErrorOr<PostResponse>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<PostResponse>> Handle(EditPostRequest request, CancellationToken cancellationToken)
    {
        if (await _forumDbContext.Users.SingleOrDefaultAsync(u => u.Id == request.PostCreatorId, cancellationToken) is null)
            return Error.NotFound(description: "user with given id not found");
        
        var post = await _forumDbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if(post is null)
            return Error.NotFound(description: "post with given id not found");

        if(post.PostCreatorId != request.PostCreatorId)
            return Error.Unauthorized(description: "current user is not allowed to change this post");

        post.Header = request.Header;
        post.Body = request.Body;

        _forumDbContext.Posts.Update(post);
        await _forumDbContext.SaveChangesAsync(cancellationToken);

        return post.Adapt<PostResponse>();
    }
}
