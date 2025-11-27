using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Token gereklidir.")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "E-posta gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Yeni þifre gereklidir.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Þifre en az 6 karakter olmalýdýr.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Þifre tekrarý gereklidir.")]
        [Compare("NewPassword", ErrorMessage = "Þifreler eþleþmiyor.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
