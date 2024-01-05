using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Forum.IntegrationTests;

public class DisableAuthenticationPolicyEvaluator : IPolicyEvaluator
{
    public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        var principal = new ClaimsPrincipal(new List<ClaimsIdentity>
        {
            new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, WebAppFactory<Program>.UserId.ToString())
            })
        });
        var authenticationTicket = new AuthenticationTicket(principal, new AuthenticationProperties(), IdentityConstants.ApplicationScheme);
        return await Task.FromResult(AuthenticateResult.Success(authenticationTicket));
    }

    public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        return await Task.FromResult(PolicyAuthorizationResult.Success());
    }
}