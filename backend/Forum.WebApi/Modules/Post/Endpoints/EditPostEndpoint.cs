﻿using ErrorOr;
using Forum.Application.Commands.Post;
using Forum.Common;
using Forum.WebApi.Modules.Post.DTOs;
using Forum.WebApi.Services;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Modules.Post.Endpoints;

public class EditPostEndpoint
{
    public static async Task<IResult> Handler(ISender sender, IUserContext userContext, Guid id, [FromBody] PostDto postDto)
    {
        var request = postDto.Adapt<EditPostRequest>();
        request.Id = id;
        request.PostCreatorId = userContext.UserId;

        var result = await sender.Send(request);

        return Results.Json(result.MatchFirst(
            value => value,
            error => 
            {
                switch(error.Type)
                {
                    case ErrorType.Unauthorized : throw new ApiException(403, error.Description);
                    case ErrorType.NotFound : throw new ApiException(404, error.Description);
                    default : throw new ApiException(500, error.Description);
                }
            }
        ));
    }
}
