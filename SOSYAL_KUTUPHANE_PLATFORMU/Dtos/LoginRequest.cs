using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Gecerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Sifre zorunludur")]
        public string Password { get; set; } = null!;
    }
}
