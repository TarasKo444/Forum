namespace Forum.Common;

public static class Throw
{
    public static void ApiException(int code, IEnumerable<ApiError> errors)
    {
        throw new ApiException(code, errors);
    }

    public static void ApiExceptionIf(bool condition, int code, IEnumerable<ApiError> details)
    {
        if(condition)
            throw new ApiException(code, details);
    }

    public static void ApiExceptionIfNull<T>(T obj, int code, ApiError details)
    {
        ApiExceptionIf(obj is null, code, [details]);
    }
}
