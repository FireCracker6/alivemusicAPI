using ALIVEMusicAPI.Models;
using ALIVEMusicAPI.Models.Entities;
using CollaborateMusicAPI.Models;
using CollaborateMusicAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollaborateMusicAPI.Contexts;

public class ApplicationDBContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDBContext()
    {
    }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }


    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public DbSet<Likes> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public DbSet<CommentClosure> CommentClosures { get; set; }

    public DbSet<CommentLikes> CommentLikes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UserProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.UserProfile)
            .HasForeignKey<UserProfile>(p => p.UserID)
            .IsRequired();

        modelBuilder.Entity<Artist>()
            .HasOne(a => a.User)
            .WithOne(u => u.Artist)
            .HasForeignKey<Artist>(a => a.UserID)
            .IsRequired(false); // UserID is optional

        modelBuilder.Entity<RefreshToken>()
             .HasOne(rt => rt.User)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(rt => rt.UserId);

        modelBuilder.Entity<SubscriptionPlan>()
     .Property(b => b.Price)
     .HasPrecision(18, 4); // Or any precision you need

        modelBuilder.Entity<Likes>()
             .HasIndex(l => new { l.UserID, l.TrackID })
             .IsUnique();

        modelBuilder.Entity<CommentLikes>()
       .HasOne(cl => cl.Comment)
       .WithMany(c => c.CommentLikes)
       .HasForeignKey(cl => cl.CommentID)
       .OnDelete(DeleteBehavior.Restrict);  // Restrict cascade on delete

        modelBuilder.Entity<CommentLikes>()
            .HasOne(cl => cl.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(cl => cl.UserID)
            .OnDelete(DeleteBehavior.Restrict);  // Restrict cascade on delete





        // Configure the Comment -> Track relationship to not cascade on delete
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Track)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TrackID)
            .OnDelete(DeleteBehavior.Restrict);  // No cascade on delete

        // Configure the Comment -> Comment (parent) relationship to not cascade on delete
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(pc => pc.ChildComments)
            .HasForeignKey(c => c.ParentCommentID)
            .OnDelete(DeleteBehavior.Restrict);  // No cascade on delete

     // Configure the Comment -> Artist relationship to not cascade on delete   

        modelBuilder.Entity<CommentClosure>()
            .HasKey(cc => new { cc.AncestorId, cc.DescendantId });

        modelBuilder.Entity<CommentClosure>()
            .HasOne(cc => cc.Ancestor)
            .WithMany(c => c.Descendants)
            .HasForeignKey(cc => cc.AncestorId)
            .OnDelete(DeleteBehavior.Restrict);  // No cascade on delete

        modelBuilder.Entity<CommentClosure>()
            .HasOne(cc => cc.Descendant)
            .WithMany(c => c.Ancestors)
            .HasForeignKey(cc => cc.DescendantId)
            .OnDelete(DeleteBehavior.Restrict);  // No cascade on delete

    }




}
