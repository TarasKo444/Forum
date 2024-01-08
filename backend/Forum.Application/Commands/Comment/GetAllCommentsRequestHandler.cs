using ErrorOr;
using Forum.Application.Commands.Comment.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Comment;

public class GetAllCommentsRequest : IRequest<ErrorOr<IEnumerable<CommentResponse>>>
{
    public Guid PostId { get; set; }
}

public class GetAllCommentsRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<GetAllCommentsRequest, ErrorOr<IEnumerable<CommentResponse>>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<IEnumerable<CommentResponse>>> Handle(GetAllCommentsRequest request,
        CancellationToken cancellationToken)
    {
        
        if (await _forumDbContext.Posts.SingleOrDefaultAsync(p => p.Id == request.PostId, cancellationToken) is null)
            return Error.NotFound(description: "post with given id not found");
        
        return _forumDbContext.Posts
            .Where(p => p.Id == request.PostId)
            .Include(p => p.Comments)
            .SelectMany(p => p.Comments)
            .Where(c => c.ParentCommentId == null)
            .AsNoTracking()
            .ProjectToType<CommentResponse>()            
            .ToList();
    }
}