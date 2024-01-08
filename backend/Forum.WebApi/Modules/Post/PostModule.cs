using Forum.WebApi.Extensions;
using Forum.WebApi.Modules.Post.Endpoints;

namespace Forum.WebApi.Modules.Post;

public class PostModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("posts", GetAllPostsEndpoint.Handler);
        endpoints.MapGet("posts/{id:guid}", GetPostEndpoint.Handler);
        endpoints.MapPost("posts", CreatePostEndpoint.Handler).RequireAuthorization();
        endpoints.MapDelete("posts/{id:guid}", DeletePostEndpoint.Handler).RequireAuthorization();
        endpoints.MapPut("posts/{id:guid}", EditPostEndpoint.Handler).RequireAuthorization();
        return endpoints;
    }
}
