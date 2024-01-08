using ErrorOr;
using Forum.Application;
using Forum.Application.Commands.Post;
using Forum.Common;
using Forum.WebApi.Services;
using Mediator;

namespace Forum.WebApi.Modules.Post.Endpoints;

public class DeletePostEndpoint
{
    public static async Task<IResult> Handler(ISender sender, IUserContext userContext, Guid id)
    {
        var result = await sender.Send(new DeletePostRequest()
        {
            Id = id,
            PostCreatorId = userContext.UserId
        });

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
