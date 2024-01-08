
using Forum.WebApi.Extensions;

namespace Forum.WebApi.Modules.Comment;

public class CommentModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }
}
