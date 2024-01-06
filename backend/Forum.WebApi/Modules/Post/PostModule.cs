
using Forum.Infrastructure;

namespace Forum.WebApi;

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

class A
{
    public static virtual void Method() {}
}