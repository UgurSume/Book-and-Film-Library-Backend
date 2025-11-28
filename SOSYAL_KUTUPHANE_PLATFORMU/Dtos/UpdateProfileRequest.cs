using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class UpdateProfileRequest
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanici adi 3-50 karakter arasinda olmalidir")]
        public string? UserName { get; set; }

        [StringLength(500, ErrorMessage = "Biyografi maksimum 500 karakter olabilir")]
        public string? Biography { get; set; }

        [Url(ErrorMessage = "Gecerli bir URL giriniz")]
        public string? AvatarUrl { get; set; }
    }
}
