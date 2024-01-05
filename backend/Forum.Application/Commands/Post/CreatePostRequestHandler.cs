using Forum.Domain.Entities;
using Forum.Infrastructure;
using Mapster;
using Mediator;

namespace Forum.Application;

public class CreatePostRequest : IRequest<PostResponse>
{
    public string Header { get; set; } = null!;
    public string Body { get; set; } = null!;
    public Guid PostCreatorId { get; set; }
}

public class CreatePostRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<CreatePostRequest, PostResponse>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<PostResponse> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var post = request.Adapt<Post>();
        post.CreatedAt = DateTime.UtcNow;

        await _forumDbContext.Posts.AddAsync(post);
        await _forumDbContext.SaveChangesAsync();

        return post.Adapt<PostResponse>();
    }
}