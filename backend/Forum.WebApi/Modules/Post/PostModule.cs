
namespace Forum.WebApi;

public class PostModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("posts", GetAllPosts.Handler);
        endpoints.MapGet("posts/{id:guid}", GetPost.Handler);
        endpoints.MapPost("posts", CreatePost.Handler).RequireAuthorization();
        endpoints.MapDelete("posts/{id:guid}", DeletePost.Handler).RequireAuthorization();
        endpoints.MapPut("posts/{id:guid}", EditPost.Handler).RequireAuthorization();
        return endpoints;
    }
}
