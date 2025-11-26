namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ListItemDto
    {

        public int ContentId { get; set; }
        public string Title { get; set; } = null!;
        public int? Year { get; set; }
        public string? CoverUrl { get; set; }
        public string Type { get; set; } = null!; // movie / book



    }
}
