
using Microsoft.AspNetCore.Identity;

namespace Forum.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime JoinedAt { get; set; }
}
