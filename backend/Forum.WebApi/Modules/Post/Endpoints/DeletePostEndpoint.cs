using ErrorOr;
using Forum.Application;
using Forum.Application.Commands.Post;
using Forum.Common;
using Forum.WebApi.Extensions;
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

        return CustomResults.Json(result);
    }
}
