using Forum.Application;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi;

public class CreatePost
{
    [Authorize]
    public static async Task<IResult> Handler(ISender sender, [FromBody] CreatePostRequest postRequest)
    {
        return Results.Json(await sender.Send(postRequest));
    }
}
