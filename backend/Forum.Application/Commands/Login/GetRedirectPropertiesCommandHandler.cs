using Mediator;
using Microsoft.AspNetCore.Authentication;

namespace Forum.Application;

public class GetRedirectPropertiesCommand : IRequest<AuthenticationProperties>
{
    public string Callback { get; set; } = null!;
    public string Scheme { get; set; } = null!;
}

public class GetRedirectPropertiesCommandHandler(IExternalAuthService externalAuthService)
    : IRequestHandler<GetRedirectPropertiesCommand, AuthenticationProperties>
{
    private readonly IExternalAuthService _externalAuthService = externalAuthService;

    public ValueTask<AuthenticationProperties> Handle(
        GetRedirectPropertiesCommand command, CancellationToken cancellationToken)
        => ValueTask.FromResult(_externalAuthService.GetRedirectProperties(command.Callback, command.Scheme));
}
