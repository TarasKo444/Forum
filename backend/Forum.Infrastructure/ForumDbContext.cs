
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
    public DbSet<UniquePostVote> UniquePostVotes { get; set; } = null!;
    public DbSet<UniqueCommentVote> UniqueCommentVotes { get; set; } = null!;

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

        builder.Entity<UniquePostVote>(vote =>
        {
            vote.HasOne(v => v.Post)
                .WithMany(p => p.VotesInfo)
                .HasForeignKey(v => v.PostId)
                .IsRequired();

            vote.HasOne(v => v.User)
                .WithMany(u => u.VotedPosts)
                .HasForeignKey(v => v.UserId)
                .IsRequired();
        });

        builder.Entity<UniqueCommentVote>(vote =>
        {
            vote.HasOne(v => v.Comment)
                .WithMany(c => c.VotesInfo)
                .HasForeignKey(v => v.CommentId)
                .IsRequired();

            vote.HasOne(v => v.User)
                .WithMany(p => p.VotedComments)
                .HasForeignKey(v => v.UserId)
                .IsRequired();
        });

        base.OnModelCreating(builder);
    }
}