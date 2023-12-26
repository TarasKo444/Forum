using Microsoft.AspNetCore.Identity;

namespace Forum.Domain.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = null!;
    public string Picture { get; set; } = null!;
}
