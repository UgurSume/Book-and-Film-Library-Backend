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
        /// Baþlýk veya açýklama aramasý
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Tür filtresi (örn: ["Action", "Drama"])
        /// </summary>
        public List<string>? Genres { get; set; }

        /// <summary>
        /// Yönetmen filtresi (filmler için)
        /// </summary>
        public string? Director { get; set; }

        /// <summary>
        /// Yazar filtresi (kitaplar için)
        /// </summary>
        public string? Author { get; set; }

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
        /// Minimum yýl
        /// </summary>
        [Range(1800, 2100, ErrorMessage = "Gecerli bir yil giriniz")]
        public int? MinYear { get; set; }

        /// <summary>
        /// Maksimum yýl
        /// </summary>
        [Range(1800, 2100, ErrorMessage = "Gecerli bir yil giriniz")]
        public int? MaxYear { get; set; }

        /// <summary>
        /// Sýralama alaný: "title", "year", "rating", "created"
        /// </summary>
        public string? SortBy { get; set; } = "created";

        /// <summary>
        /// Azalan sýralama mý?
        /// </summary>
        public bool SortDescending { get; set; } = true;

        /// <summary>
        /// Sayfa numarasý
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Sayfa numarasi 1 veya daha buyuk olmalidir")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Sayfa boyutu
        /// </summary>
        [Range(1, 100, ErrorMessage = "Sayfa boyutu 1-100 arasinda olmalidir")]
        public int PageSize { get; set; } = 20;
    }
}
