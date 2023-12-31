﻿using ErrorOr;
using Forum.Application.Services;
using Mediator;

namespace Forum.Application.Commands.Login;

public class LoginRequest : IRequest<ErrorOr<Guid>>
{
}

public class LoginRequestHandler(IExternalAuthService externalAuthService)
    : IRequestHandler<LoginRequest, ErrorOr<Guid>>
{
    private readonly IExternalAuthService _externalAuthService = externalAuthService;

    public async ValueTask<ErrorOr<Guid>> Handle(LoginRequest command, CancellationToken cancellationToken)
        => await _externalAuthService.LoginUser();  
}
