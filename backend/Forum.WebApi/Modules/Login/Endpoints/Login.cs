using Forum.Application;
using Mediator;
using Microsoft.AspNetCore.Authentication.Google;

namespace Forum.WebApi;

public class Login
{
    public static async Task<IResult> Handler(ISender sender)
    {
        var command = new GetRedirectPropertiesCommand
        {
            Callback = "api/auth/login-callback",
            Scheme = GoogleDefaults.AuthenticationScheme
        };

        var properties = await sender.Send(command);;

        return Results.Challenge(properties, [command.Scheme]);
    }
}
