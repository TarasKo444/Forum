using Forum.Infrastructure;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application;

public class GetAllPostsRequest : IRequest<IEnumerable<PostResponse>>
{
}

public class GetAllPostsRequestHandler(ForumDbContext forumDbContext)
    : IRequestHandler<GetAllPostsRequest, IEnumerable<PostResponse>>
{
    private readonly ForumDbContext _forumDbContext = forumDbContext;

    public ValueTask<IEnumerable<PostResponse>> Handle(GetAllPostsRequest request, CancellationToken cancellationToken)
        => ValueTask.FromResult(_forumDbContext.Posts.AsNoTracking().ProjectToType<PostResponse>() as IEnumerable<PostResponse>);
}
