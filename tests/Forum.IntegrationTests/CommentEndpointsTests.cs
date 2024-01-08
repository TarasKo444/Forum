
using System.Net.Http.Json;
using FluentAssertions;
using Forum.Application.Commands.Comment.Models;
using Forum.Application.Commands.Post.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.IntegrationTests;

public class CommentEndpointsTests : IClassFixture<WebAppFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly Guid _userId;
    protected readonly WebAppFactory<Program> _factory;
    
    public CommentEndpointsTests(WebAppFactory<Program> factory) 
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
    public async Task CreateCommentEndpointTest()
    {
        var responsePost = await BaseEndpointTests.CreatePost(_client);
        var post = (await responsePost.Content.ReadFromJsonAsync<PostResponse>())!;
        var responseMessage = await BaseEndpointTests.CreateComment(_client, post.Id);
        var comment = (await responseMessage.Content.ReadFromJsonAsync<CommentResponse>())!;

        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var dbContext = BaseEndpointTests.GetDbContext(scope);

        dbContext.Comments.Should().NotBeEmpty();
        dbContext.Comments.Should().HaveCount(1);
    }
}
