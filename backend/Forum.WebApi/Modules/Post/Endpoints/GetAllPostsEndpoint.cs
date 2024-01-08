using Forum.Application.Commands.Post;
using Mediator;

namespace Forum.WebApi.Modules.Post.Endpoints;

public class GetAllPostsEndpoint
{
    public static async Task<IResult> Handler(ISender sender)
    {
        return Results.Json(await sender.Send(new GetAllPostsRequest()));
    }
}
