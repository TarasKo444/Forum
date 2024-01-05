
using Microsoft.AspNetCore.Identity;

namespace Forum.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime JoinedAt { get; set; }
    public ICollection<Post> CreatedPosts { get; } = new List<Post>();
    public ICollection<Comment> WritedComments { get; } = new List<Comment>();
    public ICollection<UniquePostVote> VotedPosts { get; } = new List<UniquePostVote>();
    public ICollection<UniqueCommentVote> VotedComments { get; } = new List<UniqueCommentVote>();
}
