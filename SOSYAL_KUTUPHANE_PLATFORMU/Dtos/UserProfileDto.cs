namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class UserProfileDto
    {
  public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? AvatarUrl { get; set; }
     public string? Biography { get; set; }
        public int FollowersCount { get; set; }
   public int FollowingCount { get; set; }
        public DateTime CreatedAt { get; set; }
  
        // Giriþ yapan kullanýcý bu profili takip ediyor mu?
     public bool IsFollowing { get; set; }
  
        // Bu profil giriþ yapan kullanýcýnýn kendisi mi?
   public bool IsOwnProfile { get; set; }
    }
}
