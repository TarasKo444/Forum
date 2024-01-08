namespace Forum.Application.Commands.Comment.Models;

public class CommentResponse
{
    public Guid Id { get; set; }
    public string Body { get; set; } = null!;
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public Guid WriterId { get; set; }
    public int RepliesCount { get; set; }
}
