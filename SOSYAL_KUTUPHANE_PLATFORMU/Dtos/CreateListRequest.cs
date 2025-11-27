using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class CreateListRequest
    {
        /// <summary>
        /// Liste adý zorunludur
        /// Liste adý 1-100 karakter arasýnda olmalýdýr
        /// </summary>
        [Required(ErrorMessage = "Liste adý zorunludur")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Liste adý 1-100 karakter arasýnda olmalýdýr")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Açýklama maksimum 500 karakter olabilir
        /// </summary>
        [StringLength(500, ErrorMessage = "Açýklama maksimum 500 karakter olabilir")]
        public string? Description { get; set; }

        /// <summary>
        /// Liste tipi: "movie", "book" veya "mixed"
        /// </summary>
        public string? Type { get; set; }
    }
}
