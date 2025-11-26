namespace SOSYAL_KUTUPHANE_PLATFORMU.Dtos
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PasswordConfirm { get; set; } = null!;


    }
}
