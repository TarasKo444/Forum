using ErrorOr;
using Forum.Application.Commands.Comment.Models;
using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Comment;

public class GetCommentRequest : IRequest<ErrorOr<CommentResponse>>
{
    public Guid Id { get; set; }
}

public class GetCommentRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<GetCommentRequest, ErrorOr<CommentResponse>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<CommentResponse>> Handle(GetCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await _forumDbContext.Comments.SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        
        if (comment is null)
            return Error.NotFound(description: "comment with given id not found");

        return comment.Adapt<CommentResponse>();
    }
}
