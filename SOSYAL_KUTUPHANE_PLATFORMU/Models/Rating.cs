namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ContentId { get; set; }

        // 1-10 arası puan
        public int Score { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties (ilişkiler)
        public User User { get; set; } = null!;
        public Content Content { get; set; } = null!;
    }
}
