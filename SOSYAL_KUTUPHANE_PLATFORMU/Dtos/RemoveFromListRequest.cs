namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class RemoveFromListRequest
    {
        public int UserId { get; set; }
        public int ListId { get; set; }
        public int ContentId { get; set; }
    }
}
