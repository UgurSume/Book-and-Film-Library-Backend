namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    /// <summary>
    /// Aktivitelere yapýlan yorumlar
    /// </summary>
    public class ActivityComment
    {
        public int Id { get; set; }
        
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        
      public string Text { get; set; } = null!;
        
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Activity Activity { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
