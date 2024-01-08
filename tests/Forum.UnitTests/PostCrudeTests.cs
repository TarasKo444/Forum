using ErrorOr;
using FluentAssertions;
using Forum.Application.Commands.Post;
using Forum.Application.Commands.Post.Models;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Forum.UnitTests;

public class PostCrudeTests
{   
    private readonly Guid _userid = Guid.NewGuid();
    private readonly ForumDbContext _forumDbContext;

    public PostCrudeTests()
    {
        var contextOptions = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase("PostCrudeTests")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _forumDbContext = new(contextOptions);
        
        _forumDbContext.Database.EnsureDeleted();
        _forumDbContext.Database.EnsureCreated();
        
        _forumDbContext.Users.Add(new User() { JoinedAt = DateTime.Now, Id = _userid });
        _forumDbContext.SaveChanges();
    }

    public static async Task<ErrorOr<PostResponse>> CreatePost(ForumDbContext forumDbContext, Guid userid)
    {
        var handler = new CreatePostRequestHandler(forumDbContext);
        
        var post = await handler.Handle(new()
        {
            Header = "testheader",
            Body = "testbody",
            PostCreatorId = userid
        }, new());

        return post;
    }

    [Fact]
    public async Task CreatePostTest()
    {
        var post = await CreatePost(_forumDbContext, _userid);

        (await _forumDbContext.Posts.FirstAsync()).Id.Should().Be(post.Value.Id);
    }
    
    [Fact]
    public async Task CreatePostErrorTest()
    {
        var post = await CreatePost(_forumDbContext, Guid.NewGuid());

        post.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllPostsTest()
    {
        await CreatePost(_forumDbContext, _userid);
        await CreatePost(_forumDbContext, _userid);
        await CreatePost(_forumDbContext, _userid);

        _forumDbContext.Posts.Should().NotBeEmpty();
        _forumDbContext.Posts.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetPostTest()
    {
        var post = await CreatePost(_forumDbContext, _userid);

        var handler = new GetPostRequestHandler(_forumDbContext);
        var response = await handler.Handle(new() 
        {
            Id = post.Value.Id
        }, new());

        response.Value.Id.Should().Be(post.Value.Id);
        _forumDbContext.Posts.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task GetPostErrorTest()
    {
        await CreatePost(_forumDbContext, _userid);

        var handler = new GetPostRequestHandler(_forumDbContext);
        var response = await handler.Handle(new() 
        {
            Id = Guid.NewGuid()
        }, new());

        response.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task GetPostTestWithWrongId()
    {
        await CreatePost(_forumDbContext, _userid);

        var handler = new GetPostRequestHandler(_forumDbContext);
        var request = new GetPostRequest()
        {
            Id = Guid.NewGuid()
        };
        
        var response = await handler.Handle(request, new());

        response.Errors.Should().NotBeEmpty();
        response.Errors.First().Code.Should().Be("General.NotFound");
    }

    [Fact]
    public async Task EditPostTest()
    {
        var post = await CreatePost(_forumDbContext, _userid);

        var handler = new EditPostRequestHandler(_forumDbContext);

        var request = new EditPostRequest()
        {
            Id = post.Value.Id,
            PostCreatorId = _userid,
            Body = "1",
            Header = "2"
        };
        
        var response = await handler.Handle(request, new());

        response.Value.Id.Should().Be(post.Value.Id);
        response.Value.PostCreatorId.Should().Be(_userid);
        response.Value.Body.Should().Be("1");
    }

    [Fact]
    public async Task EditPostTestWithError1()
    {
        var handler = new EditPostRequestHandler(_forumDbContext);

        var request = new EditPostRequest()
        {
            Id = Guid.NewGuid(),
            PostCreatorId = _userid,
            Body = "1",
            Header = "2"
        };
        
        var response = await handler.Handle(request, new());

        response.FirstError.Code.Should().Be("General.NotFound");
    }

    [Fact]
    public async Task EditPostTestWithError2()
    {
        var post = await CreatePost(_forumDbContext, _userid);

        var handler = new EditPostRequestHandler(_forumDbContext);

        var request = new EditPostRequest()
        {
            Id = post.Value.Id,
            PostCreatorId = Guid.NewGuid(),
            Body = "1",
            Header = "2"
        };
        
        var response = await handler.Handle(request, new());

        response.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task DeletePostTest()
    {
        var post = await CreatePost(_forumDbContext, _userid);

        var handler = new DeletePostRequestHandler(_forumDbContext);

        var request = new DeletePostRequest()
        {
            Id = post.Value.Id,
            PostCreatorId = _userid
        };
        
        await handler.Handle(request, new());

        _forumDbContext.Posts.Should().BeEmpty();
    }
}