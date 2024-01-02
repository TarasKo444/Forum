using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi;

public class EditPost
{
    [Authorize]
    public static async Task<IResult> Handler(ISender sender, Guid id)
    {
        return Results.Ok();
    }
}
