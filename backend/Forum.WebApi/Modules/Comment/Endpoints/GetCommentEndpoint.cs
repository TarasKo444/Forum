using Forum.Application.Commands.Comment;
using Forum.WebApi.Extensions;
using Mediator;

namespace Forum.WebApi.Modules.Comment.Endpoints;

public class GetCommentEndpoint
{
    public static async Task<IResult> Handler(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetCommentRequest()
        {
            Id = id
        });
        
        return CustomResults.Json(result);
    }
}
