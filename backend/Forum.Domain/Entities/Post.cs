
namespace Forum.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Header { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public User PostCreator { get; set; } = null!;
    public Guid PostCreatorId { get; set; }
    public ICollection<Comment> Comments { get; } = new List<Comment>();
    public ICollection<UniquePostVote> VotesInfo { get; set; } = null!;
}
