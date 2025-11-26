
using System.Security.Cryptography;
using System.Text;  

namespace SOSYAL_KUTUPHANE_PLATFORMU.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash); // .NET 5+ için
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            var hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Equals(hashOfInput, passwordHash);
        }
    }


    
}
