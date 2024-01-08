using ErrorOr;
using Forum.Application.Commands.Comment.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Comment;

public class GetRepliesRequest : IRequest<ErrorOr<IEnumerable<CommentResponse>>>
{
    public Guid ParentCommentId { get; set; }
}

public class GetRepliesRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<GetRepliesRequest, ErrorOr<IEnumerable<CommentResponse>>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<IEnumerable<CommentResponse>>> Handle(GetRepliesRequest request,
        CancellationToken cancellationToken)
    {
        if (await _forumDbContext.Comments.SingleOrDefaultAsync(c => c.Id == request.ParentCommentId, cancellationToken) is null)
            return Error.NotFound(description: "comment with given id not found");

        return _forumDbContext.Comments
            .Where(c => c.ParentCommentId == request.ParentCommentId)
            .AsNoTracking()
            .ProjectToType<CommentResponse>()
            .ToList();
    }
}
