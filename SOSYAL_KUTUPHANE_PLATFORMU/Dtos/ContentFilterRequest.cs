using System.ComponentModel.DataAnnotations;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ContentFilterRequest
    {
        /// <summary>
        /// Ýçerik tipi: "movie" veya "book"
        /// </summary>
        [RegularExpression("^(movie|book)?$", ErrorMessage = "Icerik turu 'movie' veya 'book' olmalidir")]
        public string? Type { get; set; }

        /// <summary>
        /// Minimum puan (1-10 arasý)
        /// </summary>
        [Range(1, 10, ErrorMessage = "Minimum puan 1-10 arasinda olmalidir")]
        public double? MinRating { get; set; }

        /// <summary>
        /// Maksimum puan (1-10 arasý)
        /// </summary>
        [Range(1, 10, ErrorMessage = "Maksimum puan 1-10 arasinda olmalidir")]
        public double? MaxRating { get; set; }

        /// <summary>
        /// Yýl filtreleme
        /// </summary>
        [Range(1800, 2100, ErrorMessage = "Gecerli bir yil giriniz")]
        public int? Year { get; set; }

        /// <summary>
        /// Baþlangýç yýlý
        /// </summary>
        [Range(1800, 2100, ErrorMessage = "Gecerli bir yil giriniz")]
        public int? YearFrom { get; set; }

        /// <summary>
        /// Bitiþ yýlý
        /// </summary>
        [Range(1800, 2100, ErrorMessage = "Gecerli bir yil giriniz")]
        public int? YearTo { get; set; }

        /// <summary>
        /// Sýralama: "rating_desc", "rating_asc", "popular", "recent"
        /// </summary>
        [RegularExpression("^(rating_desc|rating_asc|popular|recent)$",
            ErrorMessage = "Gecerli siralama: rating_desc, rating_asc, popular, recent")]
        public string SortBy { get; set; } = "rating_desc";

        /// <summary>
        /// Sayfalama - atlanan kayýt sayýsý
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Skip 0 veya daha buyuk olmalidir")]
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Sayfalama - alýnacak kayýt sayýsý
        /// </summary>
        [Range(1, 100, ErrorMessage = "Take 1-100 arasinda olmalidir")]
        public int Take { get; set; } = 20;
    }
}
