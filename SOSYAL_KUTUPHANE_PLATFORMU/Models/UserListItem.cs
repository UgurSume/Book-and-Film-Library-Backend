namespace SOSYAL_KUTUPHANE_PLATFORMU.Models
{
    public class UserListItem
    {
        public int Id { get; set; }

        public int UserListId { get; set; }
        public int ContentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public UserList UserList { get; set; } = null!;
        public Content Content { get; set; } = null!;
    }
}
