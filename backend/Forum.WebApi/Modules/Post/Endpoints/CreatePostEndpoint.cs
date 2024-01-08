using Forum.Application;
using Forum.Application.Commands.Post;
using Forum.WebApi.Modules.Post.DTOs;
using Forum.WebApi.Services;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Modules.Post.Endpoints;

public class CreatePostEndpoint
{
    public static async Task<IResult> Handler(ISender sender, IUserContext userContext, [FromBody] PostDto postDto)
    {
        var request = postDto.Adapt<CreatePostRequest>();
        request.PostCreatorId = userContext.UserId;

        return Results.Json(await sender.Send(request));
    }
}
