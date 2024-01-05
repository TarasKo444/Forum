
namespace Forum.Domain.Entities;

public class UniqueCommentVote
{
    public Guid Id { get; set; }
    public bool IsUpVote { get; set; }
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
