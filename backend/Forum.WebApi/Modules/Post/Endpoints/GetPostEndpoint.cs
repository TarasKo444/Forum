using Forum.Application.Commands.Post;
using Forum.WebApi.Extensions;
using Mediator;

namespace Forum.WebApi.Modules.Post.Endpoints;

public class GetPostEndpoint
{
    public static async Task<IResult> Handler(ISender sender, Guid id)
    {
        var result = await sender.Send(new GetPostRequest()
        {
            Id = id
        });

        return CustomResults.Json(result);
    }
}
