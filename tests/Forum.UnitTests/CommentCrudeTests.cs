using System.Text.Json;
using ErrorOr;
using FluentAssertions;
using Forum.Application.Commands.Comment;
using Forum.Application.Commands.Comment.Models;
using Forum.Domain.Entities;
using Forum.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit.Abstractions;

namespace Forum.UnitTests;

public class CommentCrudeTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Guid _userid = Guid.NewGuid();
    private readonly ForumDbContext _forumDbContext;

    public CommentCrudeTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var contextOptions = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase("CommentCrudeTests")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _forumDbContext = new(contextOptions);

        _forumDbContext.Database.EnsureDeleted();
        _forumDbContext.Database.EnsureCreated();        
        
        _forumDbContext.Users.Add(new User() { JoinedAt = DateTime.Now, Id = _userid });
        _forumDbContext.SaveChanges();
    }

    public static async Task<ErrorOr<CommentResponse>> CreateComment(
        ForumDbContext forumDbContext, Guid postId, Guid userid, Guid? parentId = null)
    {
        var handler = new CreateCommentRequestHandler(forumDbContext);
        
        var comment = await handler.Handle(new()
        {
            Body = "test body",
            PostId = postId,
            WriterId = userid,
            ParentCommentId = parentId 
        }, new());

        return comment;
    }
    
    [Fact]
    public async Task CreateCommentTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        var comment = await CreateComment(_forumDbContext, post.Value.Id, _userid);

        (await _forumDbContext.Comments.FirstAsync()).Id.Should().Be(comment.Value.Id);
        
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(comment));
    } 
    
    [Fact]
    public async Task GetCommentTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        var comment = await CreateComment(_forumDbContext, post.Value.Id, _userid);

        var handler = new GetCommentRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            Id = comment.Value.Id
        }, new());

        result.Value.Id.Should().Be(comment.Value.Id);
    } 
    
    [Fact]
    public async Task GetCommentErrorTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        await CreateComment(_forumDbContext, post.Value.Id, _userid);

        var handler = new GetCommentRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            Id = Guid.NewGuid()
        }, new());

        result.IsError.Should().BeTrue();
    } 
    
    [Fact]
    public async Task CreateCommentErrorTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        var comment = await CreateComment(_forumDbContext, post.Value.Id, Guid.NewGuid());

        comment.IsError.Should().BeTrue();
    } 
    
    [Fact]
    public async Task GetAllCommentsTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        
        var comment1 = await CreateComment(_forumDbContext, post.Value.Id, _userid);
        await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);
        var comment22 = await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);
        var comment3 = await CreateComment(_forumDbContext, post.Value.Id, _userid, comment22.Value.Id);
        await CreateComment(_forumDbContext, post.Value.Id, _userid, comment3.Value.Id);
        await CreateComment(_forumDbContext, post.Value.Id, _userid);

        var handler = new GetAllCommentsRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            PostId = post.Value.Id
        }, new());

        var comments = result.Value.ToList();
        
        comments.Should().NotBeEmpty();
        comments.First().RepliesCount.Should().Be(2);
        comments.Should().HaveCount(2);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(comments));
    }
    
    [Fact]
    public async Task GetRepliesCommentsTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        
        var comment1 = await CreateComment(_forumDbContext, post.Value.Id, _userid);
        var comment21 = await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);
        await CreateComment(_forumDbContext, post.Value.Id, _userid, comment21.Value.Id);
        await CreateComment(_forumDbContext, post.Value.Id, _userid, comment21.Value.Id);
        await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);
        await CreateComment(_forumDbContext, post.Value.Id, _userid);

        var handler = new GetRepliesRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            ParentCommentId = comment1.Value.Id,
        }, new());

        var comments = result.Value.ToList();
        
        comments.Should().NotBeEmpty();
        comments.Should().HaveCount(2);
        comments.Where(c => c.ParentCommentId == comment1.Value.Id).Should().HaveCount(2);
        comments.First().RepliesCount.Should().Be(2);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(comments));
    }
    
    [Fact]
    public async Task EditRepliesCommentsTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        
        var comment1 = await CreateComment(_forumDbContext, post.Value.Id, _userid);
        var comment21 = await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);

        var handler = new EditCommentRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            Body = "adasdas",
            Id = comment1.Value.Id,
            WriterId = _userid
        }, new());

        result.Value.Id.Should().Be(comment1.Value.Id);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(result.Value));
    }
    
    [Fact]
    public async Task EditRepliesCommentsErrorTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        
        var comment1 = await CreateComment(_forumDbContext, post.Value.Id, _userid);
        var comment21 = await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);

        var handler = new EditCommentRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            Body = "adasdas",
            Id = Guid.NewGuid()
        }, new());

        result.IsError.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteRepliesCommentsTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        
        var comment1 = await CreateComment(_forumDbContext, post.Value.Id, _userid);
        var comment21 = await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);

        var handler = new DeleteCommentRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            Id = comment1.Value.Id,
            WriterId = _userid
        }, new());

        result.IsError.Should().BeFalse();

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(result.Value));
    }
    
    [Fact]
    public async Task DeleteRepliesCommentsErrorTest()
    {
        var post = await PostCrudeTests.CreatePost(_forumDbContext, _userid);
        
        var comment1 = await CreateComment(_forumDbContext, post.Value.Id, _userid);
        var comment21 = await CreateComment(_forumDbContext, post.Value.Id, _userid, comment1.Value.Id);

        var handler = new DeleteCommentRequestHandler(_forumDbContext);

        var result = await handler.Handle(new()
        {
            Id = Guid.NewGuid()
        }, new());

        result.IsError.Should().BeTrue();

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(result.Value));
    }
}
