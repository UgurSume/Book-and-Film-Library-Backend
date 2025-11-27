namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ContentSummaryDto
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = null!;
        public string Type { get; set; } = null!;
    public string Title { get; set; } = null!;
        public string? Description { get; set; }
 public int? Year { get; set; }
        public string? CoverUrl { get; set; }
        
// Ýstatistikler
        public double AverageRating { get; set; }
   public int RatingsCount { get; set; }
 public int ReviewsCount { get; set; }
        public int ListAddCount { get; set; } // Kaç kez listeye eklenmiþ
        
   public DateTime CreatedAt { get; set; }
  }
}
