using Forum.Application.Commands.Login;
using Forum.Common;
using Mediator;

namespace Forum.WebApi.Modules.Login.Endpoints;

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
