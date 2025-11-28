using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Token zorunludur")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Gecerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Yeni sifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Sifre en az 6 karakter olmalidir")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Sifre tekrari zorunludur")]
        [Compare("NewPassword", ErrorMessage = "Sifreler eslesmiyor")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
