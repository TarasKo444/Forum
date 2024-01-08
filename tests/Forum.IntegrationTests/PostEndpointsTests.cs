using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Forum.Application.Commands.Post.Models;
using Forum.WebApi.Modules.Post.DTOs;

namespace Forum.IntegrationTests;

public class PostEndpointsTests : IClassFixture<WebAppFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly Guid _userId;
    protected readonly WebAppFactory<Program> _factory;
    
    public PostEndpointsTests(WebAppFactory<Program> factory) 
    {
        _client = factory.CreateClient();
        _factory = factory;

        using var scope = _factory.Services.CreateScope();
        var dbContext = BaseEndpointTests.GetDbContext(scope);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        _userId = BaseEndpointTests.CreateUser(factory);
    }

    [Fact]
    public async Task GetAllPostsEndpointTest()
    { 
        await BaseEndpointTests.CreatePost(_client);
        await BaseEndpointTests.CreatePost(_client);
        await BaseEndpointTests.CreatePost(_client);

        using var scope = _factory.Services.CreateScope();
        var dbContext = BaseEndpointTests.GetDbContext(scope);

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
        var dbContext = BaseEndpointTests.GetDbContext(scope);
        
        var createResponse = await BaseEndpointTests.CreatePost(_client);
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
        var dbContext = BaseEndpointTests.GetDbContext(scope);
        
        var createResponse = await BaseEndpointTests.CreatePost(_client);
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
        var dbContext = BaseEndpointTests.GetDbContext(scope);
        
        var createResponse = await BaseEndpointTests.CreatePost(_client);
        var createdPost = (await createResponse.Content.ReadFromJsonAsync<PostResponse>())!;

        var response = await _client.DeleteAsync($"api/posts/{createdPost.Id}");

        response.IsSuccessStatusCode.Should().BeTrue();

        dbContext.Posts.Should().BeEmpty();
    }
}