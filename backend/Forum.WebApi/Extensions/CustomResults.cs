using ErrorOr;
using Forum.Common;

namespace Forum.WebApi.Extensions;

public static class CustomResults
{
    public static IResult Json<T>(ErrorOr<T> result)
    {
        return Results.Json(result.MatchFirst(
            value => value,
            error => throw GetApiException(error)));
    }

    public static ApiException GetApiException(Error error)
    {
        return error.Type switch
        {
            ErrorType.Unauthorized => new ApiException(401, error.Description),
            ErrorType.NotFound => new ApiException(404, error.Description),
            _ => new ApiException(500, error.Description)
        };
    }
    
    public static IResult Ok<T>(ErrorOr<T> result)
    {
        if (result.IsError)
        {
            throw GetApiException(result.FirstError);
        }

        return Results.Ok();
    }
}
