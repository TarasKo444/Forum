
using Forum.WebApi.Extensions;
using Forum.WebApi.Modules.Comment.Endpoints;

namespace Forum.WebApi.Modules.Comment;

public class CommentModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api/posts/{postId:guid}/comments", GetAllCommentsEndpoint.Handler);
        endpoints.MapGet("api/comment/{id:guid}", GetCommentEndpoint.Handler);
        endpoints.MapGet("api/comment/{id:guid}/replies", GetRepliesEndpoint.Handler);
        endpoints.MapPost("api/posts/{postId:guid}/comments/{parentCommentId:guid?}", CreateCommentEndpoint.Handler);
        endpoints.MapDelete("api/comments/{id:guid}", DeleteCommentEndpoint.Handler);
        endpoints.MapPut("api/comments/{id:guid}", EditCommentEndpoint.Handler);
        return endpoints;
    }
}
