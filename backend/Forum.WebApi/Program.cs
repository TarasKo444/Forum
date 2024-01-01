using Forum.Domain.Entities;
using Forum.Infrastructure;
using Forum.Application;
using Forum.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.RegisterModules();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.User.AllowedUserNameCharacters = null!;
    })
    .AddEntityFrameworkStores<ForumDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/login";
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

var api = app.MapGroup("api");
api.MapEndpoints();

api.MapGet("/auth/test", [Authorize] (HttpContext context) =>
{
    return context.User.Claims.Select(x => new { x.Type, x.Value });
});

api.MapGet("/auth/test3", async (ForumDbContext context) =>
{
    var users = context.Users.ToList(); 
    context.Users.RemoveRange(users);
    await context.SaveChangesAsync();
});

api.MapGet("/auth/test2", (ForumDbContext context) =>
{
    return context.Users.ToList();
});

api.MapGet("/auth/logout", async (SignInManager<User> signInManager) =>
{
    await signInManager.SignOutAsync();
});

app.Run();