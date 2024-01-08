using Forum.WebApi.Extensions;
using Forum.WebApi.Modules.Login.Endpoints;

namespace Forum.WebApi.Modules.Login;

public class LoginModule : IModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("auth/login", Endpoints.Login.Handler);
        endpoints.MapGet("auth/login-callback", LoginCallback.Handler);
        return endpoints;
    }
}
