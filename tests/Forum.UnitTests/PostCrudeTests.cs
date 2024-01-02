using FluentAssertions;
using Forum.Application;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Forum.UnitTests;

public class PostCrudeTests
{   
    private readonly Guid _userid = Guid.NewGuid();
    private readonly List<User> _userList = [];
    private readonly IUserContext _userContext;
    private readonly ForumDbContext _forumDbContext;

    public PostCrudeTests()
    {
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(i => i.UserId).Returns(_userid);
        _userContext = userContextMock.Object;

        var contextOptions = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase("PostCrudeTests")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _forumDbContext = new(contextOptions);

        _forumDbContext.Database.EnsureDeleted();
        _forumDbContext.Database.EnsureCreated();
    }

    public async Task<PostResponse> CreatePost()
    {
        var handler = new CreatePostRequestHandler(_forumDbContext, _userContext);
        
        var post = await handler.Handle(new()
        {
            Header = "testheader",
            Body = "testbody"
        }, new());

        return post;
    }

    [Fact]
    public async Task CreatePostTest()
    {
        var post = await CreatePost();

        (await _forumDbContext.Posts.FirstAsync()).Id.Should().Be(post.Id);
    }

    [Fact]
    public async Task GetAllPostsTest()
    {
        await CreatePost();
        await CreatePost();
        await CreatePost();

        _forumDbContext.Posts.Should().NotBeEmpty();
        _forumDbContext.Posts.Should().HaveCount(3);
    }
}