namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    /// <summary>
    /// Aktivitelere verilen beðeniler
    /// </summary>
    public class ActivityLike
    {
        public int Id { get; set; }
    
   public int ActivityId { get; set; }
  public int UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
     public Activity Activity { get; set; } = null!;
      public User User { get; set; } = null!;
    }
}
