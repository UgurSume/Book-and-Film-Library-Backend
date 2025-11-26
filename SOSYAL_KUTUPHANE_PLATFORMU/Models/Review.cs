namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ContentId { get; set; }

        public string Text { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;
        public Content Content { get; set; } = null!;
    }
}
