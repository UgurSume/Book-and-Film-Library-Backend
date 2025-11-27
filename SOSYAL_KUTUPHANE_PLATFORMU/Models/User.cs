namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? AvatarUrl { get; set; }
        public string? Biography { get; set; }

        // Takip sayıları (performans için denormalize edilmiş)
        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;
    }
}
