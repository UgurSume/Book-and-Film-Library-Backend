using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class EnsureContentRequest
    {
        [Required(ErrorMessage = "External ID zorunludur")]
        public string ExternalId { get; set; } = null!;

        [Required(ErrorMessage = "Icerik turu zorunludur")]
        [RegularExpression("^(movie|book)$", ErrorMessage = "Icerik turu 'movie' veya 'book' olmalidir")]
        public string Type { get; set; } = null!;

        [Required(ErrorMessage = "Baslik zorunludur")]
        [StringLength(500, ErrorMessage = "Baslik maksimum 500 karakter olabilir")]
        public string Title { get; set; } = null!;

        // Frontend uyumluluk için ek alanlar
        [StringLength(2000, ErrorMessage = "Aciklama maksimum 2000 karakter olabilir")]
        public string? Description { get; set; }

        public string? Overview { get; set; }  // TMDb overview alanı için alternatif
        public string? PosterPath { get; set; }  // TMDb poster_path için
        public string? ReleaseDate { get; set; }  // TMDb release_date için

        public int? Year { get; set; }

        public string? CoverUrl { get; set; }

        // *** PROJE GEREKSINIMLERI: Detaylı Alanlar ***

        // Film için
        public string? Director { get; set; }       // Yönetmen
        public List<string>? Cast { get; set; }     // Oyuncular
        public List<string>? Genres { get; set; }   // Türler

        // Kitap için
        public List<string>? Authors { get; set; }  // Yazarlar
        public int? PageCount { get; set; }         // Sayfa sayısı
    }
}
