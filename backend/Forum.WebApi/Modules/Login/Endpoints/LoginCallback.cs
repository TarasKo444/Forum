using Forum.Application.Commands.Login;
using Forum.Common;
using Forum.WebApi.Extensions;
using Mediator;

namespace Forum.WebApi.Modules.Login.Endpoints;

public class LoginCallback
{
    public static async Task<IResult> Handler(ISender sender)
    {
        var result = await sender.Send(new LoginRequest());

        return CustomResults.Json(result);
    }
}
