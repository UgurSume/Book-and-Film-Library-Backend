using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Content> Contents { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        public DbSet<UserList> UserLists { get; set; } = null!;
        public DbSet<UserListItem> UserListItems { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;
        
        public DbSet<UserFollower> UserFollowers { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

        public DbSet<ActivityLike> ActivityLikes { get; set; } = null!;
        public DbSet<ActivityComment> ActivityComments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
            base.OnModelCreating(modelBuilder);

            // UserFollower için composite unique index
 modelBuilder.Entity<UserFollower>()
         .HasIndex(uf => new { uf.FollowerId, uf.FollowingId })
        .IsUnique();

    // Self-referencing relationship'leri yapılandır
            modelBuilder.Entity<UserFollower>()
     .HasOne(uf => uf.Follower)
  .WithMany()
    .HasForeignKey(uf => uf.FollowerId)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollower>()
    .HasOne(uf => uf.Following)
          .WithMany()
             .HasForeignKey(uf => uf.FollowingId)
.OnDelete(DeleteBehavior.Restrict);
         
      // ActivityLike için composite unique index (Bir kullanıcı bir aktiviteyi bir kez beğenebilir)
         modelBuilder.Entity<ActivityLike>()
        .HasIndex(al => new { al.ActivityId, al.UserId })
        .IsUnique();
   }
    }
}
