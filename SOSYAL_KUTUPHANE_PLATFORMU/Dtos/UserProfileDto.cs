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

        // Kullanýcý Ýstatistikleri
        public int TotalRatings { get; set; }   // Toplam puanlama sayýsý
        public int TotalReviews { get; set; }      // Toplam yorum sayýsý
        public int TotalLists { get; set; }        // Toplam özel liste sayýsý
        public int TotalActivities { get; set; }   // Toplam aktivite sayýsý
    }
}
