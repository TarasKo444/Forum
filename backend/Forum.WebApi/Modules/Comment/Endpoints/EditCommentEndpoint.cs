using Forum.Application.Commands.Comment;
using Forum.WebApi.Extensions;
using Forum.WebApi.Modules.Comment.DTOs;
using Forum.WebApi.Services;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Modules.Comment.Endpoints;

public class EditCommentEndpoint
{
    public static async Task<IResult> Handler(ISender sender, Guid id, 
        IUserContext userContext, [FromBody] CommentDto commentDto)
    {
        var request = commentDto.Adapt<EditCommentRequest>();
        request.WriterId = userContext.UserId;
        request.Id = id;
        
        var result = await sender.Send(request);
        
        return CustomResults.Json(result);
    }
}
