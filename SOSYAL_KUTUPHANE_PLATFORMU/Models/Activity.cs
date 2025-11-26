namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class Activity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ContentId { get; set; }

        // "rating", "review", "add_to_list" gibi değerler
        public string ActivityType { get; set; } = null!;

        // Listeye ekleme için (add_to_list)
        public int? ListId { get; set; }

        // Puanlama için
        public int? Score { get; set; }

        // Yorum veya kısa açıklama için
        public string? Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Content Content { get; set; } = null!;
    }
}
