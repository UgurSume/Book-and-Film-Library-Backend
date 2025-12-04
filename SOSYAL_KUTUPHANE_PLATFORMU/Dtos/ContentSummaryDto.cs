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

        // Detaylý alanlar
        public string? Director { get; set; }        // Film yönetmeni
        public List<string>? Genres { get; set; }    // Türler
        public List<string>? Authors { get; set; }   // Kitap yazarlarý
        public int? PageCount { get; set; }           // Kitap sayfa sayýsý

        // Ýstatistikler
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public int ReviewsCount { get; set; }
        public int ListAddCount { get; set; } // Kaç kez listeye eklenmiþ

        public DateTime CreatedAt { get; set; }
    }
}
