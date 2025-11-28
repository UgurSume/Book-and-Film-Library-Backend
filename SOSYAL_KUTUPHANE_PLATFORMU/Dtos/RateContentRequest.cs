using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
  public class RateContentRequest
    {
  [Required(ErrorMessage = "Icerik ID zorunludur")]
     public int ContentId { get; set; }

[Required(ErrorMessage = "Puan zorunludur")]
     [Range(1, 10, ErrorMessage = "Puan 1-10 arasinda olmalidir")]
 public int Score { get; set; }
    }
}
