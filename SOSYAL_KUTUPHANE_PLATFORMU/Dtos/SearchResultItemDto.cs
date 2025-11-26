namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class SearchResultItemDto
    {
        public string ExternalId { get; set; } = null!;
        public string Type { get; set; } = null!; // "movie" veya "book"

        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Year { get; set; }
        public string? CoverUrl { get; set; }
    }
}
