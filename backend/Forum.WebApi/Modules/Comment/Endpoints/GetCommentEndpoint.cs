using Mediator;

namespace Forum.WebApi.Modules.Comment.Endpoints;

public class GetCommentEndpoint
{
    public static async Task<IResult> Handler(ISender sender)
    {
        return Results.Json(new {});
    }
}
