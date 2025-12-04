namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class SearchResultItemDto
    {
        public string ExternalId { get; set; } = null!;
        public string Type { get; set; } = null!; // "movie" veya "book"

        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Year { get; set; }
        public string? CoverUrl { get; set; }

        // *** PROJE METNI GEREKSINIMLERI ***

        // Film için ek alanlar
        public string? Director { get; set; }      // Yönetmen
        public List<string>? Cast { get; set; }    // Oyuncular
        public List<string>? Genres { get; set; }  // Türler

        // Kitap için ek alanlar
        public List<string>? Authors { get; set; }    // Yazarlar
        public int? PageCount { get; set; }// Sayfa sayısı
    }
}
