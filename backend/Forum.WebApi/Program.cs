using System.Security.Claims;
using Forum.Common.Options;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GoogleOAuthOptions>(
    builder.Configuration.GetSection(nameof(GoogleOAuthOptions)));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientSecret = builder.Configuration["GoogleOAuthOptions:ClientSecret"]!;
        options.ClientId = builder.Configuration["GoogleOAuthOptions:ClientId"]!;
        
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.Scope.Add("openid");

        options.SaveTokens = true;
        
        options.ClaimActions.Clear();
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey("picture", "picture");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
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

app.MapGet("/", (HttpContext context) => context.User.Claims.Select(x => new { x.Type, x.Value }));

app.MapGet("/finish-auth", async (HttpContext context, ForumDbContext forumDbContext) =>
{
    var claims = context.User.Claims.ToDictionary(x => x.Type, x => x.Value);

    var email = claims[ClaimTypes.Email];

    var user = await forumDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

    if (user is null)
    {
        user = new User()
        {
            Sub = claims[ClaimTypes.NameIdentifier],
            Name = claims[ClaimTypes.Name],
            Picture = claims["picture"],
            Email = email,
        };

        await forumDbContext.Users.AddAsync(user);
        await forumDbContext.SaveChangesAsync();
    }
});

app.MapGet("/test", (ForumDbContext context) =>
{
    var result = context.Users;
    return result;
});

app.MapGet("/login", () => Results.Challenge(new()
    {
        RedirectUri = "/finish-auth"
    }, [GoogleDefaults.AuthenticationScheme]));

app.MapGet("/authtest", [Authorize](HttpContext context) => context.User.Claims.Select(x => new { x.Type, x.Value }).ToList());

app.Run();