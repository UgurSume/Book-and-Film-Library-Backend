using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;
using SOSYAL_KUTUPHANE_PLATFORMU.Helpers;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;
using SOSYAL_KUTUPHANE_PLATFORMU.Services;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
  public class AuthController : ControllerBase
    {
   private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

  public AuthController(
       ApplicationDbContext context, 
            IJwtService jwtService,
  IEmailService emailService)
        {
            _context = context;
            _jwtService = jwtService;
    _emailService = emailService;
   }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
          if (!ModelState.IsValid)
     return BadRequest(ApiResponse.FailResponse("Geçersiz veri", ModelState.Values
         .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
                 .ToList()));

            // Email veya kullanıcı adı kullanımda mı?
     var exists = await _context.Users
      .AnyAsync(u => u.Email == model.Email || u.UserName == model.UserName);

      if (exists)
                return BadRequest(ApiResponse.FailResponse("Bu kullanıcı adı veya email zaten kullanılıyor."));

         var user = new User
    {
      UserName = model.UserName,
                Email = model.Email,
            PasswordHash = PasswordHelper.HashPassword(model.Password),
      CreatedAt = DateTime.UtcNow
            };

         _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Varsayılan listeler
            var defaultLists = new List<UserList>
  {
     new UserList { UserId = user.Id, Name = "İzlediklerim", Type = "movie", IsDefault = true },
         new UserList { UserId = user.Id, Name = "İzlenecekler", Type = "movie", IsDefault = true },
       new UserList { UserId = user.Id, Name = "Okuduklarım", Type = "book", IsDefault = true },
  new UserList { UserId = user.Id, Name = "Okunacaklar", Type = "book", IsDefault = true }
};

 _context.UserLists.AddRange(defaultLists);
            await _context.SaveChangesAsync();

  // JWT Token üret
   var token = _jwtService.GenerateToken(user);

return Ok(ApiResponse<object>.SuccessResponse(new
        {
    userId = user.Id,
     userName = user.UserName,
         token = token
            }, "Kayıt başarılı."));
  }

   // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
 return BadRequest(ApiResponse.FailResponse("Geçersiz veri", ModelState.Values
.SelectMany(v => v.Errors)
             .Select(e => e.ErrorMessage)
  .ToList()));

         var user = await _context.Users
              .FirstOrDefaultAsync(u => u.Email == model.Email);

          if (user == null)
      return Unauthorized(ApiResponse.FailResponse("Email veya şifre hatalı."));

            var isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);
  if (!isPasswordValid)
   return Unauthorized(ApiResponse.FailResponse("Email veya şifre hatalı."));

     // JWT Token üret
            var token = _jwtService.GenerateToken(user);

     return Ok(ApiResponse<object>.SuccessResponse(new
   {
    userId = user.Id,
       userName = user.UserName,
       token = token
      }, "Giriş başarılı."));
     }

        // POST: api/auth/forgot-password
     [HttpPost("forgot-password")]
   public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
    return BadRequest(ApiResponse.FailResponse("Geçersiz veri", ModelState.Values
            .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage)
           .ToList()));

     var user = await _context.Users
           .FirstOrDefaultAsync(u => u.Email == model.Email);

     // Güvenlik: Email bulunamasa bile başarılı mesajı dön
            if (user == null)
  {
         return Ok(ApiResponse.SuccessResponse("Eğer bu email kayıtlıysa, şifre sıfırlama linki gönderildi."));
   }

            // Önceki kullanılmamış tokenları sil
       var oldTokens = await _context.PasswordResetTokens
      .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
.ToListAsync();

     _context.PasswordResetTokens.RemoveRange(oldTokens);

         // Yeni token oluştur
            var resetToken = new PasswordResetToken
   {
        UserId = user.Id,
           Token = Guid.NewGuid().ToString(),
       ExpiresAt = DateTime.UtcNow.AddHours(1),
           CreatedAt = DateTime.UtcNow
    };

          _context.PasswordResetTokens.Add(resetToken);
     await _context.SaveChangesAsync();

 // Email gönder
    try
     {
   await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken.Token, user.UserName);
       }
   catch (Exception ex)
            {
       Console.WriteLine($"Email gönderilirken hata: {ex.Message}");
         }

            return Ok(ApiResponse.SuccessResponse("Eğer bu email kayıtlıysa, şifre sıfırlama linki gönderildi."));
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
        if (!ModelState.IsValid)
   return BadRequest(ApiResponse.FailResponse("Geçersiz veri", ModelState.Values
    .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
          .ToList()));

            var resetToken = await _context.PasswordResetTokens
      .Include(t => t.User)
     .FirstOrDefaultAsync(t => t.Token == model.Token);

       if (resetToken == null)
        return BadRequest(ApiResponse.FailResponse("Geçersiz veya süresi dolmuş token."));

            if (resetToken.IsUsed)
 return BadRequest(ApiResponse.FailResponse("Bu token zaten kullanılmış."));

  if (resetToken.ExpiresAt < DateTime.UtcNow)
       return BadRequest(ApiResponse.FailResponse("Token'ın süresi dolmuş. Lütfen yeni bir şifre sıfırlama talebi oluşturun."));

     // Şifreyi güncelle
        resetToken.User.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
    resetToken.IsUsed = true;

            _context.Users.Update(resetToken.User);
            _context.PasswordResetTokens.Update(resetToken);
 await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse("Şifreniz başarıyla güncellendi. Artık giriş yapabilirsiniz."));
        }

        // GET: api/auth/validate-reset-token/{token}
        [HttpGet("validate-reset-token/{token}")]
        public async Task<IActionResult> ValidateResetToken(string token)
        {
 var resetToken = await _context.PasswordResetTokens
          .FirstOrDefaultAsync(t => t.Token == token);

       if (resetToken == null || resetToken.IsUsed || resetToken.ExpiresAt < DateTime.UtcNow)
            {
        return BadRequest(ApiResponse<object>.FailResponse("Token geçersiz veya süresi dolmuş.", 
    new List<string> { "isValid: false" }));
      }

 return Ok(ApiResponse<object>.SuccessResponse(new { isValid = true }, "Token geçerli."));
     }
    }
}
