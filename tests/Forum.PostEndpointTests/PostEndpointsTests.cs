using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Forum.Application.Commands.Post.Models;
using Forum.IntegrationTests;
using Forum.WebApi.Modules.Post.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.PostEndpointTests;

public class PostEndpointsTests : IClassFixture<WebAppFactory>
{
    protected readonly HttpClient _client;
    protected readonly Guid _userId = WebAppFactory.UserId;
    protected readonly WebAppFactory _factory;

    public PostEndpointsTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        var dbContext = Shared.Init(scope);
    }

    [Fact]
    public async Task GetAllPostsEndpointTest()
    { 
        using var scope = _factory.Services.CreateScope();
        var dbContext = Shared.GetDbContext(scope);
        
        await Shared.CreatePost(_client);
        await Shared.CreatePost(_client);
        await Shared.CreatePost(_client);

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
        var dbContext = Shared.GetDbContext(scope);
        
        var createResponse = await Shared.CreatePost(_client);
        var createdPost = (await createResponse.Content.ReadFromJsonAsync<PostResponse>())!;

        var response = await _client.GetAsync($"api/posts/{createdPost.Id}");

        response.IsSuccessStatusCode.Should().BeTrue();

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        post.Should().NotBeNull();

        dbContext.Posts.Should().NotBeEmpty();
        dbContext.Posts.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task GetPostEndpointErrorTest()
    { 
        using var scope = _factory.Services.CreateScope();
        var dbContext = Shared.GetDbContext(scope);

        var response = await _client.GetAsync($"api/posts/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditPostEndpointTest()
    { 
        using var scope = _factory.Services.CreateScope();
        var dbContext = Shared.GetDbContext(scope);
        
        var createResponse = await Shared.CreatePost(_client);
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
        var dbContext = Shared.GetDbContext(scope);
        
        var createResponse = await Shared.CreatePost(_client);
        var createdPost = (await createResponse.Content.ReadFromJsonAsync<PostResponse>())!;

        var response = await _client.DeleteAsync($"api/posts/{createdPost.Id}");

        response.IsSuccessStatusCode.Should().BeTrue();

        dbContext.Posts.Should().BeEmpty();
    }
}