
using Forum.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure;

public class ForumDbContext(DbContextOptions options) : IdentityDbContext(options)
{
}