
namespace Forum.WebApi;

public class LoginModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/login", Login.Handler);
        endpoints.MapGet("auth/login-callback", LoginCallback.Handler);
        return endpoints;
    }
}
