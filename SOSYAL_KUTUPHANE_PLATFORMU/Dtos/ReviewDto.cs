namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Kullanıcı profiline gitmek için
        public string UserName { get; set; } = null!;
        public string? UserAvatarUrl { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
