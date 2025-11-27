using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Services
{
    public interface IJwtService
    {
    string GenerateToken(User user);
   int? ValidateToken(string token);
  }
}
