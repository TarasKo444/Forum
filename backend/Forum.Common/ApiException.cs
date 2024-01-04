namespace Forum.Common;

public record ApiError(string Details, string? Property = null)
{
    public string Message
    { 
        get 
        {
            if(Property is null)
                return Details;
            
            return string.Format(Details, Property);
        }
    }
}

public class ApiException : Exception
{
    public readonly DateTime Timestamp = DateTime.UtcNow; 
    public IEnumerable<ApiError> Errors { get; set; }
    public int Status { get; set; }

    public ApiException(int status, IEnumerable<ApiError> errors)
    {
        Status = status;
        Errors = errors;
    }

    public ApiException(int status, string error)
    {
        Status = status;
        Errors = [new(error)];
    }
}
