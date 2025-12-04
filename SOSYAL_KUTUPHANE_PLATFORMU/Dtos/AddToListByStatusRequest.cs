using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class AddToListByStatusRequest
    {
     [Required(ErrorMessage = "Content ID zorunludur")]
        public int ContentId { get; set; }

        [Required(ErrorMessage = "Status zorunludur")]
        [RegularExpression("^(watched|to_watch|read|to_read)$", 
   ErrorMessage = "Status 'watched', 'to_watch', 'read' veya 'to_read' olmalýdýr")]
     public string Status { get; set; } = null!;
    }
}
