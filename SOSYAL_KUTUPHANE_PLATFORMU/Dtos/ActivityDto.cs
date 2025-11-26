namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class ActivityDto
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;
        public string ContentTitle { get; set; } = null!;
        public string ActivityType { get; set; } = null!;

        public int? Score { get; set; }          // rating için
        public string? Text { get; set; }        // review için
        public string? ListName { get; set; }    // add_to_list için

        public DateTime CreatedAt { get; set; }
    }
}
