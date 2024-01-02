using Mediator;
using Microsoft.AspNetCore.Authorization;

namespace Forum.WebApi;

public class DeletePost
{
    [Authorize]
    public static async Task<IResult> Handler(ISender sender, Guid id)
    {
        return Results.Ok();
    }
}
