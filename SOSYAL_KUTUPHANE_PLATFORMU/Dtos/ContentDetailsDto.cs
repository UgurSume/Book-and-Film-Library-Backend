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

        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }

        public List<ReviewDto> Reviews { get; set; } = new();
    }
}
