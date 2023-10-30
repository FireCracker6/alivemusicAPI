using CollaborateMusicAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollaborateMusicAPI.Contexts;

public class ApplicationDBContext : DbContext
{

    public ApplicationDBContext()
    {
    }
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Users>()
            .HasOne(u => u.UserProfile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserID);

        modelBuilder.Entity<Users>().HasData(
            new Users
            {
                Id = 1,
                Email = "testuser@example.com",
                PasswordHash = "TestPasswordHash",
                OAuthId = "OauthTest",
                OAuthProvider = "TestProvider",
                CreatedDate = DateTime.UtcNow
            }
          );

       modelBuilder.Entity<UserProfile>().HasData(
                      new UserProfile
                      {
                UserProfileID = 1,
                FullName = "Test User",
                Bio = "This is a test bio.",
                ProfilePicturePath = null,
                Location = "Test City",
                WebsiteURL = "https://example.com",
                UserID = 1
            }
                             );
   
    }
    
}
