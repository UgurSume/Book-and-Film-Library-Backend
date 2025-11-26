namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class AddToListRequest
    {
        public int UserId { get; set; }
        public int ListId { get; set; }
        public int ContentId { get; set; }
    }
}
