using ErrorOr;
using Forum.Application.Commands.Comment.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Comment;

public class CreateCommentRequest : IRequest<ErrorOr<CommentResponse>>
{
    public string Body { get; set; } = null!;
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Guid WriterId { get; set; }
}

public class CreateCommentRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<CreateCommentRequest, ErrorOr<CommentResponse>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<CommentResponse>> Handle(CreateCommentRequest request, CancellationToken cancellationToken)
    {
        if (await _forumDbContext.Users.SingleOrDefaultAsync(u => u.Id == request.WriterId, cancellationToken) is null)
            return Error.NotFound(description: "user with given id not found");
        
        if (await _forumDbContext.Posts.SingleOrDefaultAsync(p => p.Id == request.PostId, cancellationToken) is null)
            return Error.NotFound(description: "post with given id not found");
        
        if (request.ParentCommentId is not null && 
            await _forumDbContext.Comments.SingleOrDefaultAsync(c => c.Id == request.ParentCommentId, cancellationToken) is null)
            return Error.NotFound(description: "comment with given id not found");
        
        var comment = request.Adapt<Domain.Entities.Comment>();
        comment.CreatedAt = DateTime.UtcNow;
            
        await _forumDbContext.AddAsync(comment, cancellationToken);
        await _forumDbContext.SaveChangesAsync(cancellationToken);

        return comment.Adapt<CommentResponse>();
    }
}
