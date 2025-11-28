using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class AddToListRequest
    {
        [Required(ErrorMessage = "Liste ID zorunludur")]
        public int ListId { get; set; }

        [Required(ErrorMessage = "Icerik ID zorunludur")]
        public int ContentId { get; set; }
    }
}
