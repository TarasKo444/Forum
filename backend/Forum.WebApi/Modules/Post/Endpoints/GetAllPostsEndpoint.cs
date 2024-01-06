using Forum.Application;
using Mediator;

namespace Forum.WebApi;

public class GetAllPostsEndpoint
{
    public static async Task<IResult> Handler(ISender sender)
    {
        return Results.Json(await sender.Send(new GetAllPostsRequest()));
    }
}
