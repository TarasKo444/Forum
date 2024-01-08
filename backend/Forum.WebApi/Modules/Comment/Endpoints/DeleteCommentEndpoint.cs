using Forum.Application.Commands.Comment;
using Forum.WebApi.Extensions;
using Forum.WebApi.Services;
using Mediator;

namespace Forum.WebApi.Modules.Comment.Endpoints;

public class DeleteCommentEndpoint
{
    public static async Task<IResult> Handler(ISender sender, IUserContext userContext,Guid id)
    {
        var result = await sender.Send(new DeleteCommentRequest()
        {
            Id = id,
            WriterId = userContext.UserId
        });
        
        return CustomResults.Ok(result);
    }
}
