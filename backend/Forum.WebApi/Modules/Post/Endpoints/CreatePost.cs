using Forum.Application;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi;

public class CreatePost
{
    public static async Task<IResult> Handler(ISender sender, IUserContext userContext, [FromBody] PostDto postDto)
    {
        var request = postDto.Adapt<CreatePostRequest>();
        request.PostCreatorId = userContext.UserId;

        return Results.Json(await sender.Send(request));
    }
}
