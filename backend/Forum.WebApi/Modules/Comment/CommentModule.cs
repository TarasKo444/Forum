
using Forum.WebApi.Extensions;
using Forum.WebApi.Modules.Comment.Endpoints;

namespace Forum.WebApi.Modules.Comment;

public class CommentModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("posts/{postId:guid}/comments", GetAllCommentsEndpoint.Handler);
        endpoints.MapGet("comment/{id:guid}", GetCommentEndpoint.Handler);
        endpoints.MapGet("comment/{id:guid}/replies", GetRepliesEndpoint.Handler);
        endpoints.MapPost("posts/{postId:guid}/comments/{parentCommentId:guid?}", CreateCommentEndpoint.Handler).RequireAuthorization();
        endpoints.MapDelete("comments/{id:guid}", DeleteCommentEndpoint.Handler).RequireAuthorization();;
        endpoints.MapPut("comments/{id:guid}", EditCommentEndpoint.Handler).RequireAuthorization();;
        return endpoints;
    }
}
