using ErrorOr;
using Forum.Application.Commands.Post.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Post;

public class CreatePostRequest : IRequest<ErrorOr<PostResponse>>
{
    public string Header { get; set; } = null!;
    public string Body { get; set; } = null!;
    public Guid PostCreatorId { get; set; }
}

public class CreatePostRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<CreatePostRequest, ErrorOr<PostResponse>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<PostResponse>> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        if (await _forumDbContext.Users.SingleOrDefaultAsync(u => u.Id == request.PostCreatorId, cancellationToken) is null)
            return Error.NotFound(description: "user with given id not found");
        
        var post = request.Adapt<Domain.Entities.Post>();
        post.CreatedAt = DateTime.UtcNow;

        await _forumDbContext.Posts.AddAsync(post, cancellationToken);
        await _forumDbContext.SaveChangesAsync(cancellationToken);

        return post.Adapt<PostResponse>();
    }
}