using System.Net.Http.Json;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Forum.WebApi;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Forum.Application;
using Forum.Application.Commands.Post.Models;
using Forum.WebApi.Modules.Post.DTOs;

namespace Forum.IntegrationTests;

public class PostEndpointsTests : IClassFixture<WebAppFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Guid _userId;
    private readonly WebAppFactory<Program> _factory;

    public PostEndpointsTests(WebAppFactory<Program> factory) 
    {
        _client = factory.CreateClient();
        _factory = factory;

        using var scope = _factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        _userId = CreateUser();
    } 

    private ForumDbContext GetDbContext(IServiceScope scope)
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ForumDbContext>();

        return dbContext;
    }

    private Guid CreateUser()
    {
        var user = new User
        {
            Id = WebAppFactory<Program>.UserId,
            Email = "test@gmail.com",
            UserName = "test"
        };

        using var scope = _factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);

        dbContext.Users.AddAsync(user);
        dbContext.SaveChanges();

        return user.Id;
    }

    private async Task<HttpResponseMessage> CreatePost()
    {
        var response = await _client.PostAsJsonAsync<PostDto>("/api/posts", new()
        {
            Header = "header",
            Body = "body"
        });

        return response!;
    }

    [Fact]
    public async Task CreatePostEndpointTest()
    {
        var response = await CreatePost();

        response.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);

        dbContext.Posts.Should().NotBeEmpty();
        dbContext.Posts.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllPostsEndpointTest()
    { 
        await CreatePost();
        await CreatePost();
        await CreatePost();

        using var scope = _factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);

        var response = await _client.GetAsync("api/posts");

        response.IsSuccessStatusCode.Should().BeTrue();
        
        var posts = (await response.Content.ReadFromJsonAsync<PostResponse[]>())!;

        posts.Should().NotBeEmpty();
        posts.Should().HaveCount(3);
        dbContext.Posts.Should().NotBeEmpty();
        dbContext.Posts.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetPostEndpointTest()
    { 
        using var scope = _factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);
        
        var createResponse = await CreatePost();
        var createdPost = (await createResponse.Content.ReadFromJsonAsync<PostResponse>())!;

        var response = await _client.GetAsync($"api/posts/{createdPost.Id}");

        response.IsSuccessStatusCode.Should().BeTrue();

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        post.Should().NotBeNull();

        dbContext.Posts.Should().NotBeEmpty();
        dbContext.Posts.Should().HaveCount(1);
    }

    [Fact]
    public async Task EditPostEndpointTest()
    { 
        using var scope = _factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);
        
        var createResponse = await CreatePost();
        var createdPost = (await createResponse.Content.ReadFromJsonAsync<PostResponse>())!;

        var response = await _client.PutAsJsonAsync<PostDto>($"api/posts/{createdPost.Id}", new()
        {
            Header = "edited1",
            Body = "edited2"
        });

        response.IsSuccessStatusCode.Should().BeTrue();

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        post.Should().NotBeNull();

        post!.Id.Should().Be(createdPost.Id);
        post!.Header.Should().NotBe(createdPost.Body);
        post!.Body.Should().NotBe(createdPost.Header);
        dbContext.Posts.Should().NotBeEmpty();
        dbContext.Posts.Should().HaveCount(1);
    }

    [Fact]
    public async Task DeletePostEndpointTest()
    { 
        using var scope = _factory.Services.CreateScope();
        var dbContext = GetDbContext(scope);
        
        var createResponse = await CreatePost();
        var createdPost = (await createResponse.Content.ReadFromJsonAsync<PostResponse>())!;

        var response = await _client.DeleteAsync($"api/posts/{createdPost.Id}");

        response.IsSuccessStatusCode.Should().BeTrue();

        dbContext.Posts.Should().BeEmpty();
    }
}