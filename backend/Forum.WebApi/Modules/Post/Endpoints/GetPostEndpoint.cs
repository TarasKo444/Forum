using Forum.Application;
using Forum.Common;
using Mediator;

namespace Forum.WebApi;

public class GetPostEndpoint
{
    public static async Task<IResult> Handler(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetPostRequest()
        {
            Id = id
        });

        return Results.Json(result.MatchFirst(
            value => value,
            error => throw new ApiException(404, error.Description)
        ));
    }
}
