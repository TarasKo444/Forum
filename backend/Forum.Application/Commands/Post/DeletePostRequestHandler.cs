﻿using ErrorOr;
using Forum.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Commands.Post;

public class DeletePostRequest : IRequest<ErrorOr<Unit>>
{
    public Guid Id { get; set; }
    public Guid PostCreatorId { get; set; }
}

public class DeletePostRequestHandler (ForumDbContext forumDbContext)
    : IRequestHandler<DeletePostRequest, ErrorOr<Unit>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public async ValueTask<ErrorOr<Unit>> Handle(DeletePostRequest request, CancellationToken cancellationToken)
    {
        if (await _forumDbContext.Users.SingleOrDefaultAsync(u => u.Id == request.PostCreatorId, cancellationToken) is null)
            return Error.NotFound(description: "user with given id not found");
        
        var post = await _forumDbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if(post is null)
            return Error.NotFound(description: "post with given id not found");

        if(post.PostCreatorId != request.PostCreatorId)
            return Error.Unauthorized(description: "current user is not allowed to change this post");

        _forumDbContext.Posts.Remove(post);
        await _forumDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
