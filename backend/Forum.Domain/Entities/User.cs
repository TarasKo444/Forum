
namespace Forum.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Sub { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Picture { get; set; } = null!;
    public string Email { get; set; } = null!;
}
