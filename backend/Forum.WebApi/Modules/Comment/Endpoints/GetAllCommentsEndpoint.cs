using Forum.Application.Commands.Comment;
using Forum.WebApi.Extensions;
using Mediator;

namespace Forum.WebApi.Modules.Comment.Endpoints;

public class GetAllCommentsEndpoint
{
    public static async Task<IResult> Handler(ISender sender, Guid postId)
    {
        var result = await sender.Send(new GetAllCommentsRequest()
        {
            PostId = postId
        });
        
        return CustomResults.Json(result);
    }
}
