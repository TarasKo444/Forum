using ErrorOr;
using Forum.Application.Commands.Comment.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Comment;

public class EditCommentRequest : IRequest<ErrorOr<CommentResponse>>
{
    public Guid Id { get; set; }
    public Guid WriterId { get; set; }
    public string Body { get; set; } = null!;
}

public class EditCommentRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<EditCommentRequest, ErrorOr<CommentResponse>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<CommentResponse>> Handle(EditCommentRequest request, CancellationToken cancellationToken)
    {
        if (await _forumDbContext.Users.SingleOrDefaultAsync(u => u.Id == request.WriterId, cancellationToken) is null)
            return Error.NotFound(description: "user with given id not found");
        
        var comment = await _forumDbContext.Comments.SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        
        if(comment is null)
            return Error.NotFound("comment with given id not found");

        if(comment.WriterId != request.WriterId)
            return Error.Unauthorized("user not allowed to edit this comment");
        
        comment.Body = request.Body;

        _forumDbContext.Comments.Update(comment);
        await _forumDbContext.SaveChangesAsync(cancellationToken);
        
        return comment.Adapt<CommentResponse>();
    }
}
