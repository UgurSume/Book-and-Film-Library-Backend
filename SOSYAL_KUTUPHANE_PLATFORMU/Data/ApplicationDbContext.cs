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

    }
}
