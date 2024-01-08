using Forum.Application.Services;
using Mediator;
using Microsoft.AspNetCore.Authentication;

namespace Forum.Application.Commands.Login;

public class RedirectPropertiesRequest : IRequest<AuthenticationProperties>
{
    public string Callback { get; set; } = null!;
    public string Scheme { get; set; } = null!;
}

public class GetRedirectPropertiesRequestHandler(IExternalAuthService externalAuthService)
    : IRequestHandler<RedirectPropertiesRequest, AuthenticationProperties>
{
    private readonly IExternalAuthService _externalAuthService = externalAuthService;

    public ValueTask<AuthenticationProperties> Handle(
        RedirectPropertiesRequest command, CancellationToken cancellationToken)
        => ValueTask.FromResult(_externalAuthService.GetRedirectProperties(command.Callback, command.Scheme));
}
