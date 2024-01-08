using ErrorOr;
using Forum.Common;

namespace Forum.WebApi.Extensions;

public static class CustomResults
{
    public static IResult Json<T>(ErrorOr<T> result)
    {
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
