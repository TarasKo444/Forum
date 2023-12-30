using System.Security.Claims;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters = null;
    })
    .AddEntityFrameworkStores<ForumDbContext>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = GoogleDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    /*.AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(6);
        options.SlidingExpiration = true;
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
    })*/
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["GoogleOAuthOptions:ClientId"]!;
        options.ClientSecret = builder.Configuration["GoogleOAuthOptions:ClientSecret"]!;
        
        options.ClaimActions.MapJsonKey("picture", "picture");
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/login", async (HttpContext context, SignInManager<IdentityUser> signInManager, string? returnUrl = null) =>
{
    var url = "/auth/login-callback";
    if (returnUrl is not null)
        url += $"?returnUrl={returnUrl}";
    
    var properties = signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, url);
    
    return Results.Challenge(properties);
});

app.MapGet("/auth/login-callback", async (
    HttpContext context,
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    string? returnUrl = null) =>
{
    var info = await signInManager.GetExternalLoginInfoAsync();

    if (info is null)
    {
        throw new NullReferenceException(nameof(info));    
    }
    
    var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);

    if (!result.Succeeded)
    {
        var user = new IdentityUser
        {
            UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
        };
        
        await userManager.CreateAsync(user);

        var userResult = await userManager.AddLoginAsync(user, info);

        if (!userResult.Succeeded)
        {
            throw new InvalidOperationException(userResult.ToString());
        }
        
        await signInManager.SignInAsync(user, isPersistent: false);
    }
    
    return returnUrl is not null ? Results.LocalRedirect(returnUrl) : Results.Ok();
});

app.MapGet("/auth/test", [Authorize] (HttpContext context) =>
{
    return context.User.Claims.Select(x => new { x.Type, x.Value });
});

app.MapGet("/auth/test3", async (ForumDbContext context) =>
{
    var users = context.Users.ToList(); 
    context.Users.RemoveRange(users);
    await context.SaveChangesAsync();
});

app.MapGet("/auth/test2", (ForumDbContext context) =>
{
    return context.Users.ToList();
});

app.MapGet("/auth/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
});

app.Run();