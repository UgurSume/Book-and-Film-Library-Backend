using System.Collections.Generic;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ContentDetailsDto
    {
        public int Id { get; set; }

        public string ExternalId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Year { get; set; }
        public string? CoverUrl { get; set; }

        // *** PROJE GEREKSINIMLERI: Detaylı Alanlar ***
      
        // Film için
        public string? Director { get; set; }
        public List<string>? Cast { get; set; }
        public List<string>? Genres { get; set; }
        
        // Kitap için
        public List<string>? Authors { get; set; }
        public int? PageCount { get; set; }

        // Platform istatistikleri
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public int ReviewsCount { get; set; }
        public int ListAddCount { get; set; }  // Kaç kullanıcı listeye eklemiş

        // Giriş yapmış kullanıcının bu içerikle ilgili durumu (nullable - anonim kullanıcı için)
        public int? CurrentUserRating { get; set; }  // Kullanıcının verdiği puan (1-10)
        public bool HasUserReviewed { get; set; }// Kullanıcı yorum yapmış mı?
        public UserLibraryStatusDto? UserLibraryStatus { get; set; }  // Hangi listelerde?

        public List<ReviewDto> Reviews { get; set; } = new();
    }

    /// <summary>
    /// Kullanıcının bu içeriği hangi listelerine eklediğini gösterir
    /// </summary>
    public class UserLibraryStatusDto
    {
        public bool IsInWatchedList { get; set; }      // İzlediklerim'de mi?
        public bool IsInToWatchList { get; set; }       // İzlenecekler'de mi?
        public bool IsInReadList { get; set; }// Okuduklarım'da mı?
        public bool IsInToReadList { get; set; }        // Okunacaklar'da mı?
        public List<string> CustomLists { get; set; } = new();  // Özel listelerde (liste isimleri)
    }
}
