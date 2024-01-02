using Forum.Domain.Entities;

namespace Forum.Domain;

public class Comment
{
    public Guid Id { get; set; }
    public string Body { get; set; } = null!;
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Post Post { get; set; } = null!;
    public Guid PostId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; } = new List<Comment>();
    public Guid? ParentCommentId { get; set; }
    public User Writer { get; set; } = null!;
    public Guid WriterId { get; set; }
}
