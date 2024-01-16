using System.Net.Http.Json;
using FluentAssertions;
using Forum.Application.Commands.Comment.Models;
using Forum.Application.Commands.Post.Models;
using Forum.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.CommentEndpointTests;

public class CommentEndpointsTests : IClassFixture<WebAppFactory>
{
    protected readonly HttpClient _client;
    protected readonly Guid _userId = WebAppFactory.UserId;
    protected readonly WebAppFactory _factory;

    public CommentEndpointsTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
        using var scope = _factory.Services.CreateScope();
        Shared.Init(scope);
    }

    [Fact]
    public async Task CreateCommentEndpointTest()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = Shared.GetDbContext(scope);
        
        var responsePost = await Shared.CreatePost(_client);
        var post = (await responsePost.Content.ReadFromJsonAsync<PostResponse>())!;
        var responseMessage = await Shared.CreateComment(_client, post.Id);
        var comment = (await responseMessage.Content.ReadFromJsonAsync<CommentResponse>())!;

        responseMessage.IsSuccessStatusCode.Should().BeTrue();
        

        dbContext.Comments.Should().NotBeEmpty();
        dbContext.Comments.Should().HaveCount(1);
    }
}
