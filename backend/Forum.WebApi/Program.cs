using System.Security.Claims;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters = null!;
    })
    .AddEntityFrameworkStores<ForumDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
});

builder.Services.AddAuthentication(IdentityConstants.ExternalScheme)
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

app.MapGet("/auth/login", (HttpContext context, SignInManager<User> signInManager, string? returnUrl = null) =>
{
    var url = "/auth/login-callback";
    if (returnUrl is not null)
        url += $"?returnUrl={returnUrl}";

    var properties = signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, url);

    return Results.Challenge(properties, [GoogleDefaults.AuthenticationScheme]);
});

app.MapGet("/auth/login-callback", async (
    HttpContext context,
    SignInManager<User> signInManager,
    UserManager<User> userManager,
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
        var user = new User
        {
            UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
            JoinedAt = DateTime.UtcNow
        };
        
        await userManager.CreateAsync(user);

        var userResult = await userManager.AddLoginAsync(user, info);
        
        if (!userResult.Succeeded)
        {
            throw new InvalidOperationException(userResult.ToString());
        }
        
        await userManager.AddClaimAsync(user, info.Principal.FindFirst("picture")!);

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

app.MapGet("/auth/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
});

app.Run();