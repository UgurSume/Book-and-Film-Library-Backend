namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        
      public int UserId { get; set; }
      
    /// <summary>
        /// Rastgele oluþturulan token (GUID)
        /// </summary>
   public string Token { get; set; } = null!;
      
        /// <summary>
   /// Token'ýn son kullaným tarihi (genelde 1 saat)
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        
        /// <summary>
     /// Token kullanýldý mý?
        /// </summary>
public bool IsUsed { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      
        // Navigation property
        public User User { get; set; } = null!;
    }
}
