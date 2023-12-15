using Forum.Common.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Constants = Forum.Common.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GoogleOAuthOptions>(
    builder.Configuration.GetSection(nameof(GoogleOAuthOptions)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

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

app.MapGet("/auth", (HttpContext context, IOptions<GoogleOAuthOptions> _options) =>
{
    var redirectUri = $"https://{context.Request.Host}/auth/code";

    var queryParams = new Dictionary<string, string>()
    {
        { "client_id", _options.Value.ClientId },
        { "redirect_uri", redirectUri },
        { "response_type", "code " },
        { "scope", "profile" },
        { "access_type", "offline" }
    };

    var url = QueryHelpers.AddQueryString(Constants.AuthUri, queryParams!);
    return Results.Redirect(url);
});


app.MapGet("/auth/code", async (HttpContext context,
    string code, 
    HttpClient httpClient,
    IOptions<GoogleOAuthOptions> _options) =>
{
    var redirectUri = $"https://{context.Request.Host}/auth/code";

    var queryParams = new Dictionary<string, string>()
    {
        { "client_id", _options.Value.ClientId },
        { "client_secret", _options.Value.ClientSecret },
        { "code", code },
        { "grant_type", "authorization_code" },
        { "redirect_uri", redirectUri }
    };

    var url = QueryHelpers.AddQueryString(Constants.TokenUri, queryParams!);
    var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

    var response = await httpClient.SendAsync(httpRequestMessage);
    var json = await response.Content.ReadFromJsonAsync<object>();

    return json;
});

app.Run();
