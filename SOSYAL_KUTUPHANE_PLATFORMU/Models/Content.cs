namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class Content
    {
        public int Id { get; set; }

        // Harici API'den gelen ID (TMDb ID, Google Books ID vs.)
        public string ExternalId { get; set; } = null!;

        // "movie", "book" gibi tür bilgisi
        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        // Yayın yılı (kitap/film yılı)
        public int? Year { get; set; }

        // Poster / kapak görseli URL'si
        public string? CoverUrl { get; set; }

        // *** PROJE METNI GEREKSINIMLERI ***

        // Film için ek alanlar
        public string? Director { get; set; }  // Yönetmen
        public string? Cast { get; set; }  // Oyuncular (JSON array string olarak)
        public string? Genres { get; set; }    // Türler (JSON array string olarak: ["Action", "Drama"])
        
        // Kitap için ek alanlar
        public string? Authors { get; set; }   // Yazarlar (JSON array string olarak)
        public int? PageCount { get; set; }    // Sayfa sayısı

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigasyonlar – DB şemasını değiştirmez, o yüzden yeni migration gerekmiyor
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
