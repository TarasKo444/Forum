namespace Forum.Common;

public record ApiError(string Message) 
{
    // public string? Property { get; set; }
};

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
}
