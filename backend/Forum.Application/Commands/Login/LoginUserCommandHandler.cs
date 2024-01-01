
using Mediator;

namespace Forum.Application;

public class LoginUserCommand : IRequest<Guid>
{
}

public class LoginUserCommandHandler(IExternalAuthService externalAuthService)
    : IRequestHandler<LoginUserCommand, Guid>
{
    private readonly IExternalAuthService _externalAuthService = externalAuthService;

    public async ValueTask<Guid> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        => await _externalAuthService.LoginUser();
}
