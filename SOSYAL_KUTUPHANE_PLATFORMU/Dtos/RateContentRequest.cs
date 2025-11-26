namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class RateContentRequest
    {
        public int UserId { get; set; }      // Şimdilik body'den alıyoruz, ileride JWT'ye taşırız
        public int ContentId { get; set; }
        public int Score { get; set; }       // 1–10 arası

    }
}
