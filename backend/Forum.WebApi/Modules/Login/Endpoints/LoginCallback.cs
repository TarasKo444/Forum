using Forum.Application;
using Forum.Common;
using Mediator;

namespace Forum.WebApi;

public class LoginCallback
{
    public static async Task<IResult> Handler(ISender sender)
    {
        var result = await sender.Send(new LoginRequest());

        return Results.Json(result.MatchFirst(
            value => value,
            error => throw new ApiException(401, error.Description)
        ));
    }
}
