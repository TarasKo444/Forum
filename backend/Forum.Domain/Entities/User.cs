
using Microsoft.AspNetCore.Identity;

namespace Forum.Domain.Entities;

public class User : IdentityUser
{
    public DateTime JoinedAt { get; set; }
}
