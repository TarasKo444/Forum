using ErrorOr;
using Forum.Application;
using Forum.Common;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi;

public class EditPost
{
    [Authorize]
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
