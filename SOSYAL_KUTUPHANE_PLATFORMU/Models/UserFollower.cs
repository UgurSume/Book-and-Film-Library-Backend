namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    /// <summary>
    /// Kullanýcýlar arasý takip iliþkisi
    /// </summary>
    public class UserFollower
    {
      public int Id { get; set; }

        /// <summary>
        /// Takip eden kullanýcý ID'si
        /// </summary>
        public int FollowerId { get; set; }

  /// <summary>
        /// Takip edilen kullanýcý ID'si
        /// </summary>
     public int FollowingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
   public User Follower { get; set; } = null!;
        public User Following { get; set; } = null!;
    }
}
