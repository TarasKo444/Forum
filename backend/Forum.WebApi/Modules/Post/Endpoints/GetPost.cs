using Mediator;

namespace Forum.WebApi;

public class GetPost
{
    public static async Task<IResult> Handler(ISender sender, Guid id)
    {
        return Results.Ok();
    }
}
