namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ActivityDto
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;
        public string? UserAvatarUrl { get; set; }
        public int UserId { get; set; }  // Frontend için kullanıcı profiline gitmek için
      
        public int ContentId { get; set; }  // Frontend için içerik detayına gitmek için
        public string ContentTitle { get; set; } = null!;
        public string? ContentCoverUrl { get; set; }
        public string? ContentType { get; set; }
        
        public string ActivityType { get; set; } = null!;

        public int? Score { get; set; }        // rating için
        public string? Text { get; set; }   // review için (tam metin)
        public string? TextExcerpt { get; set; } // review için (feed'de gösterilecek kısa özet - 150-200 karakter)
        public bool HasMoreText { get; set; }    // Devamı var mı? "...daha fazlasını oku" göstermek için
        public string? ListName { get; set; }    // add_to_list için

        public DateTime CreatedAt { get; set; }

        // Etkileşim istatistikleri
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        
        // Giriş yapan kullanıcı bu aktiviteyi beğendi mi?
        public bool IsLikedByCurrentUser { get; set; }
    }
}
