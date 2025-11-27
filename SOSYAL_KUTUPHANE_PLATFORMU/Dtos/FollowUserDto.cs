namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class FollowUserDto
    {
        public int Id { get; set; }
  public string UserName { get; set; } = null!;
     public string? AvatarUrl { get; set; }
        public string? Biography { get; set; }
   public int FollowersCount { get; set; }
  public int FollowingCount { get; set; }
  public bool IsFollowing { get; set; } // Giriþ yapan kullanýcý bunu takip ediyor mu?
    }
}
