using System.Security.Claims;
using Forum.Common.Options;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GoogleOAuthOptions>(
    builder.Configuration.GetSection(nameof(GoogleOAuthOptions)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ForumDbContext>()
    .AddDefaultTokenProviders();

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

        //options.SaveTokens = true;
        
        options.ClaimActions.Clear();
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey("picture", "picture");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
    });

builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(builder.Configuration);

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext context, string returnUrl = "") =>
{
    Console.WriteLine(returnUrl);
    return context.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
});

app.MapGet("/login", () => Results.Challenge(new GoogleChallengeProperties()
{
    RedirectUri = "/",
},new List<string> { GoogleDefaults.AuthenticationScheme }));

app.MapGet("/logout", () => Results.SignOut(new AuthenticationProperties()
{
    RedirectUri = "/"
},new List<string> { CookieAuthenticationDefaults.AuthenticationScheme }));

app.MapGet("/test", (ForumDbContext context) =>
{
    return context.Users;
});

app.MapGet("/authtest", [Authorize]() => "Authorized");

app.Run();