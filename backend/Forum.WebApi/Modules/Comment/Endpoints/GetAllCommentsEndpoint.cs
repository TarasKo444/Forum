using Mediator;

namespace Forum.WebApi.Modules.Comment.Endpoints;

public class GetAllCommentsEndpoint
{
    public static async Task<IResult> Handler(ISender sender)
    {
        return Results.Json(new {});
    }
}
