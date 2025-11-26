namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsDefault { get; set; }

        public List<ListItemDto> Items { get; set; } = new();
    }
}
