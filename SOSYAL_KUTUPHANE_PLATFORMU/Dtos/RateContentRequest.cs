using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class RateContentRequest
    {
        [Required(ErrorMessage = "İçerik ID zorunludur")]
        public int ContentId { get; set; }

        [Required(ErrorMessage = "Puan zorunludur")]
        [Range(1, 10, ErrorMessage = "Puan 1-10 arasında olmalıdır")]
        public int Score { get; set; }       // 1–10 arası

    }
}
