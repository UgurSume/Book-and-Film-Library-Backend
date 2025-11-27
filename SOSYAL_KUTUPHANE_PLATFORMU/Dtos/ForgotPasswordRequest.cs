using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "E-posta zorunludur")]
      [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = null!;
  }
}
