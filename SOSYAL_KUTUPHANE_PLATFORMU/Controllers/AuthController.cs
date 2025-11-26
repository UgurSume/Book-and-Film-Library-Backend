using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOSYAL_KUTUPHANE_PLATFORMU.Data;
using SOSYAL_KUTUPHANE_PLATFORMU.Dtos;
using SOSYAL_KUTUPHANE_PLATFORMU.Helpers;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.PasswordConfirm)
                return BadRequest("Şifre ve şifre tekrarı aynı olmalı.");

            // Email veya kullanıcı adı kullanımda mı?
            var exists = await _context.Users
                .AnyAsync(u => u.Email == model.Email || u.UserName == model.UserName);

            if (exists)
                return BadRequest("Bu kullanıcı adı veya email zaten kullanılıyor.");

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
    new UserList
    {
        UserId = user.Id,
        Name = "İzlediklerim",
        Type = "movie",
        IsDefault = true
    },
    new UserList
    {
        UserId = user.Id,
        Name = "İzlenecekler",
        Type = "movie",
        IsDefault = true
    },
    new UserList
    {
        UserId = user.Id,
        Name = "Okuduklarım",
        Type = "book",
        IsDefault = true
    },
    new UserList
    {
        UserId = user.Id,
        Name = "Okunacaklar",
        Type = "book",
        IsDefault = true
    }
};

            _context.UserLists.AddRange(defaultLists);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Kayıt başarılı.",
                userId = user.Id,
                userName = user.UserName
            });

        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return Unauthorized("Email veya şifre hatalı.");

            var isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized("Email veya şifre hatalı.");

            // Şimdilik sadece basit mesaj dönüyoruz.
            // Sonraki adımda burada JWT token üreteceğiz.
            return Ok(new
            {
                message = "Giriş başarılı.",
                userId = user.Id,
                userName = user.UserName
            });
        }
    }
}
