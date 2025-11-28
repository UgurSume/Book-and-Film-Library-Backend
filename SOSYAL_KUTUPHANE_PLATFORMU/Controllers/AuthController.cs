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

        /// <summary>
     /// Kullanici kayit
        /// </summary>
     [HttpPost("kayit")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
     if (!ModelState.IsValid)
          return BadRequest(ApiResponse.FailResponse("Gecersiz veri", ModelState.Values
         .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
      .ToList()));

         var exists = await _context.Users
          .AnyAsync(u => u.Email == model.Email || u.UserName == model.UserName);

            if (exists)
        return BadRequest(ApiResponse.FailResponse("Bu kullanici adi veya email zaten kullaniliyor."));

            var user = new User
      {
     UserName = model.UserName,
      Email = model.Email,
     PasswordHash = PasswordHelper.HashPassword(model.Password),
        CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

         var defaultLists = new List<UserList>
     {
                new UserList { UserId = user.Id, Name = "Izlediklerim", Type = "movie", IsDefault = true },
  new UserList { UserId = user.Id, Name = "Izlenecekler", Type = "movie", IsDefault = true },
            new UserList { UserId = user.Id, Name = "Okuduklarim", Type = "book", IsDefault = true },
         new UserList { UserId = user.Id, Name = "Okunacaklar", Type = "book", IsDefault = true }
  };

       _context.UserLists.AddRange(defaultLists);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(ApiResponse<object>.SuccessResponse(new
         {
 userId = user.Id,
       userName = user.UserName,
     token = token
            }, "Kayit basarili."));
        }

    // POST: api/auth/login
[HttpPost("giris")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
if (!ModelState.IsValid)
          return BadRequest(ApiResponse.FailResponse("Gecersiz veri", ModelState.Values
                 .SelectMany(v => v.Errors)
   .Select(e => e.ErrorMessage)
       .ToList()));

   var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Email == model.Email);

    if (user == null)
        return Unauthorized(ApiResponse.FailResponse("Email veya sifre hatali."));

       var isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);
  if (!isPasswordValid)
 return Unauthorized(ApiResponse.FailResponse("Email veya sifre hatali."));

            var token = _jwtService.GenerateToken(user);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
           userId = user.Id,
    userName = user.UserName,
            token = token
         }, "Giris basarili."));
        }

        /// <summary>
        /// Sifremi unuttum
        /// </summary>
        [HttpPost("sifremi-unuttum")]
     public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
 {
     if (!ModelState.IsValid)
         return BadRequest(ApiResponse.FailResponse("Gecersiz veri", ModelState.Values
    .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
             .ToList()));

    var user = await _context.Users
    .FirstOrDefaultAsync(u => u.Email == model.Email);

         if (user == null)
       {
          return Ok(ApiResponse.SuccessResponse("Eger bu email kayitliysa, sifre sifirlama linki gonderildi."));
          }

          var oldTokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
    .ToListAsync();

            _context.PasswordResetTokens.RemoveRange(oldTokens);

     var resetToken = new PasswordResetToken
   {
      UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
         ExpiresAt = DateTime.UtcNow.AddHours(1),
           CreatedAt = DateTime.UtcNow
     };

            _context.PasswordResetTokens.Add(resetToken);
       await _context.SaveChangesAsync();

  try
         {
     await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken.Token, user.UserName);
            }
            catch (Exception ex)
        {
       Console.WriteLine($"Email gonderilirken hata: {ex.Message}");
            }

            return Ok(ApiResponse.SuccessResponse("Eger bu email kayitliysa, sifre sifirlama linki gonderildi."));
     }

   /// <summary>
        /// Sifre sifirla
   /// </summary>
   [HttpPost("sifre-sifirla")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
   if (!ModelState.IsValid)
    return BadRequest(ApiResponse.FailResponse("Gecersiz veri", ModelState.Values
 .SelectMany(v => v.Errors)
      .Select(e => e.ErrorMessage)
       .ToList()));

            var resetToken = await _context.PasswordResetTokens
  .Include(t => t.User)
     .FirstOrDefaultAsync(t => t.Token == model.Token);

            if (resetToken == null)
     return BadRequest(ApiResponse.FailResponse("Gecersiz veya suresi dolmus token."));

  if (resetToken.IsUsed)
                return BadRequest(ApiResponse.FailResponse("Bu token zaten kullanilmis."));

 if (resetToken.ExpiresAt < DateTime.UtcNow)
         return BadRequest(ApiResponse.FailResponse("Token'in suresi dolmus. Lutfen yeni bir sifre sifirlama talebi olusturun."));

  resetToken.User.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
     resetToken.IsUsed = true;

      _context.Users.Update(resetToken.User);
            _context.PasswordResetTokens.Update(resetToken);
  await _context.SaveChangesAsync();

  return Ok(ApiResponse.SuccessResponse("Sifreniz basariyla guncellendi. Artik giris yapabilirsiniz."));
        }

        /// <summary>
      /// Sifre sifirlama token dogrulama
        /// </summary>
        [HttpGet("token-dogrula/{token}")]
        public async Task<IActionResult> ValidateResetToken(string token)
        {
  var resetToken = await _context.PasswordResetTokens
      .FirstOrDefaultAsync(t => t.Token == token);

            if (resetToken == null || resetToken.IsUsed || resetToken.ExpiresAt < DateTime.UtcNow)
            {
      return BadRequest(ApiResponse<object>.FailResponse("Token gecersiz veya suresi dolmus.", 
  new List<string> { "isValid: false" }));
     }

            return Ok(ApiResponse<object>.SuccessResponse(new { isValid = true }, "Token gecerli."));
        }
    }
}
