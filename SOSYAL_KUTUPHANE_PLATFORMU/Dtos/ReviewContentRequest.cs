namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ReviewContentRequest
    {
        public int UserId { get; set; }
        public int ContentId { get; set; }
        public string Text { get; set; } = null!;
    }
}
