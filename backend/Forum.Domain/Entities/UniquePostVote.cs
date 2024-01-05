
namespace Forum.Domain.Entities;

public class UniquePostVote
{
    public Guid Id { get; set; }
    public bool IsUpVote { get; set; }
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
