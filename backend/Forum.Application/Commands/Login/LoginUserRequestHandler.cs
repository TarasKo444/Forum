
using Mediator;

namespace Forum.Application;

public class LoginRequest : IRequest<Guid>
{
}

public class LoginRequestHandler(IExternalAuthService externalAuthService)
    : IRequestHandler<LoginRequest, Guid>
{
    private readonly IExternalAuthService _externalAuthService = externalAuthService;

    public async ValueTask<Guid> Handle(LoginRequest command, CancellationToken cancellationToken)
        => await _externalAuthService.LoginUser();
}
