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

        [StringLength(2000, ErrorMessage = "Aciklama maksimum 2000 karakter olabilir")]
        public string? Description { get; set; }

        [Range(1800, 2100, ErrorMessage = "Gecerli bir yil giriniz")]
        public int? Year { get; set; }

        [Url(ErrorMessage = "Gecerli bir URL giriniz")]
        public string? CoverUrl { get; set; }
    }
}
