using Mediator;

namespace Forum.WebApi.Modules.Comment.Endpoints;

public class DeleteCommentEndpoint
{
    public static async Task<IResult> Handler(ISender sender)
    {
        return Results.Json(new {});
    }
}
