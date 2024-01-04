using System.Security.Claims;
using ErrorOr;
using Forum.Common;
using Forum.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Forum.Application;

public interface IExternalAuthService 
{
    public AuthenticationProperties GetRedirectProperties(string callback, string scheme);
    public Task<ErrorOr<Guid>> LoginUser();
}

public class ExternalAuthService(
    IHttpContextAccessor httpContextAccessor,
    SignInManager<User> signInManager) : IExternalAuthService
{
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public AuthenticationProperties GetRedirectProperties(string callback, string scheme)
        => _signInManager.ConfigureExternalAuthenticationProperties(scheme, callback);

    public async Task<ErrorOr<Guid>> LoginUser() 
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        
        if(info is null)
            return Error.Unauthorized(description: "external login failed");

        var result = await _signInManager.ExternalLoginSignInAsync(
            info!.LoginProvider, info.ProviderKey, false, true);

        if (!result.Succeeded)
        {
            var user = new User
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                JoinedAt = DateTime.UtcNow
            };
            
            await _signInManager.UserManager.CreateAsync(user);

            var userResult = await _signInManager.UserManager.AddLoginAsync(user, info);
        
            if(!userResult.Succeeded)
                return Error.Unauthorized(description: "user creating failed");

            await _signInManager.UserManager.AddClaimAsync(user, info.Principal.FindFirst("picture")!);

            await _signInManager.SignInAsync(user, isPersistent: false);

            return user.Id;
        }
        
        return Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
