namespace Forum.Application.Commands.Post.Models;

public class PostResponse
{
    public Guid Id { get; set; }
    public string Header { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid PostCreatorId { get; set; }
}
