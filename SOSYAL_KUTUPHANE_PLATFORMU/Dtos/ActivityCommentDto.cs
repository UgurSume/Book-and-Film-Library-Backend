namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ActivityCommentDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        
        public int UserId { get; set; }
     public string UserName { get; set; } = null!;
  public string? UserAvatarUrl { get; set; }
        
public string Text { get; set; } = null!;
        
    public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
        
        // Giriþ yapan kullanýcý bu yorumu yapan mý?
    public bool IsOwnComment { get; set; }
    }
}
