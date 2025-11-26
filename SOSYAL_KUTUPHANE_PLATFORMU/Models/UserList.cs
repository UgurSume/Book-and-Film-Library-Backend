namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class UserList
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        // Örn: "İzlediklerim", "İzlenecekler", "Okuduklarım", "Okunacaklar"
        public string Name { get; set; } = null!;

        // "movie" / "book" / "mixed" vb.
        public string Type { get; set; } = null!;

        // Varsayılan liste mi? (true = sistem tarafından oluşturuldu)
        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;

        public ICollection<UserListItem> Items { get; set; } = new List<UserListItem>();
    }
}
