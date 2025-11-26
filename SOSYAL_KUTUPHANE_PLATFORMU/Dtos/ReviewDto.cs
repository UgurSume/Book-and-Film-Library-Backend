namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

    }
}
