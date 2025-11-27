using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class EnsureContentRequest
    {
        [Required(ErrorMessage = "External ID zorunludur")]
        public string ExternalId { get; set; } = null!;

        [Required(ErrorMessage = "İçerik türü zorunludur")]
        [RegularExpression("^(movie|book)$", ErrorMessage = "İçerik türü 'movie' veya 'book' olmalıdır")]
        public string Type { get; set; } = null!;

        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(500, ErrorMessage = "Başlık maksimum 500 karakter olabilir")]
        public string Title { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Açıklama maksimum 2000 karakter olabilir")]
        public string? Description { get; set; }

        [Range(1800, 2100, ErrorMessage = "Geçerli bir yıl giriniz")]
        public int? Year { get; set; }

        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        public string? CoverUrl { get; set; }
    }
}
