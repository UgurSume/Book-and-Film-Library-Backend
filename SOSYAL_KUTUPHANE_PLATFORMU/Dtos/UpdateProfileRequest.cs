using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class UpdateProfileRequest
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanýcý adý 3-50 karakter arasýnda olmalýdýr")]
        public string? UserName { get; set; }

        [StringLength(500, ErrorMessage = "Biyografi maksimum 500 karakter olabilir")]
        public string? Biography { get; set; }

        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        public string? AvatarUrl { get; set; }
    }
}
