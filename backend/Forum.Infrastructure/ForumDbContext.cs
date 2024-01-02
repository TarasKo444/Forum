
using Forum.Domain;
using Forum.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure;

public class ForumDbContext(DbContextOptions options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Post>(post =>
        {
            post.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .HasPrincipalKey(c => c.Id)
                .IsRequired(false);

            post.HasOne(p => p.PostCreator)
                .WithMany(u => u.CreatedPosts)
                .HasForeignKey(p => p.PostCreatorId)
                .IsRequired();
        });

        builder.Entity<Comment>(comment =>
        {
            comment.HasOne(c => c.Writer)
                .WithMany(u => u.WritedComments)
                .HasForeignKey(c => c.WriterId)
                .IsRequired();

            comment.HasMany(c => c.Replies)
                .WithOne(r => r.ParentComment)
                .HasForeignKey(r => r.ParentCommentId)
                .IsRequired(false);
        });

        base.OnModelCreating(builder);
    }
}