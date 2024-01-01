using Forum.Application;
using Mediator;

namespace Forum.WebApi;

public class LoginCallback
{
    public static async Task<IResult> Handler(ISender sender)
        => Results.Json(await sender.Send(new LoginUserCommand()));
}
