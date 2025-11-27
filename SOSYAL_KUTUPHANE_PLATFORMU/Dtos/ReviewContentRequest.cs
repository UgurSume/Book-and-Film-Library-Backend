using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ReviewContentRequest
    {
        [Required(ErrorMessage = "İçerik ID zorunludur")]
        public int ContentId { get; set; }

        [Required(ErrorMessage = "Yorum metni zorunludur")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Yorum 10-2000 karakter arasında olmalıdır")]
        public string Text { get; set; } = null!;
    }
}
