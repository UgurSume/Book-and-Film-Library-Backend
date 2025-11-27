using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class AddActivityCommentRequest
    {
   [Required(ErrorMessage = "Yorum metni gereklidir.")]
        [MinLength(1, ErrorMessage = "Yorum en az 1 karakter olmalýdýr.")]
        [MaxLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir.")]
        public string Text { get; set; } = null!;
 }
}
