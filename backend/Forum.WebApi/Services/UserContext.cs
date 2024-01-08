using System.Security.Claims;
using Forum.Common;

namespace Forum.WebApi.Services;

public interface IUserContext
{
    public ClaimsPrincipal User { get; }
    public Guid UserId { get; }
}

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext!.User;

    public Guid UserId 
    { 
        get 
        {
            if (Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                return id;

            throw new ApiException(401, "user not authorized");
        }
    }
}
