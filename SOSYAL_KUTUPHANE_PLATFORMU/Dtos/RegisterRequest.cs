using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Kullanici adi zorunludur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanici adi 3-50 karakter arasinda olmalidir")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Gecerli bir e-posta adresi giriniz")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Sifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Sifre en az 6 karakter olmalidir")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Sifre tekrari zorunludur")]
        [Compare("Password", ErrorMessage = "Sifreler eslesmiyor")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
