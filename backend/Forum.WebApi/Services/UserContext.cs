using System.Security.Claims;
using Forum.Common;

namespace Forum.Application;

public interface IUserContext
{
    public ClaimsPrincipal User { get; }
    public Guid UserId { get; }
}

public class UserContext(HttpContext httpContext) : IUserContext
{
    private readonly HttpContext _httpContext = httpContext;

    public ClaimsPrincipal User => _httpContext.User;

    public Guid UserId 
    { 
        get 
        {
            if (Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                return id;

            throw new ApiException(401, [new("user not authorized")]);
        }
    }
}
