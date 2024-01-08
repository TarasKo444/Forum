using ErrorOr;
using Forum.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Comment;

public class DeleteCommentRequest : IRequest<ErrorOr<Unit>>
{
    public Guid Id { get; set; }
    public Guid WriterId { get; set; }
}

public class DeleteCommentRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<DeleteCommentRequest, ErrorOr<Unit>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<Unit>> Handle(DeleteCommentRequest request, CancellationToken cancellationToken)
    {
        if (await _forumDbContext.Users.SingleOrDefaultAsync(u => u.Id == request.WriterId, cancellationToken) is null)
            return Error.NotFound(description: "user with given id not found");
        
        var comment = await _forumDbContext.Comments.SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        
        if(comment is null)
            return Error.NotFound("comment with given id not found");
        
        if(comment.WriterId != request.WriterId)
            return Error.Unauthorized("user not allowed to delete this comment");

        _forumDbContext.Comments.Remove(comment);
        await _forumDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
