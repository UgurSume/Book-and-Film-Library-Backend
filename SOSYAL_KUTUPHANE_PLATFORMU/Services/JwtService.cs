using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SOSYAL_KUTUPHANE_PLATFORMU.Models;

namespace SOSYAL_KUTUPHANE_PLATFORMU.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

      public JwtService(IConfiguration configuration)
        {
      _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
   var secretKey = _configuration["Jwt:SecretKey"]
   ?? throw new Exception("JWT SecretKey tanýmlý deðil.");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "1440");

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
   var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

          var claims = new[]
  {
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.Email, user.Email),
     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

 var token = new JwtSecurityToken(
      issuer: issuer,
             audience: audience,
           claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
             signingCredentials: credentials
         );

  return new JwtSecurityTokenHandler().WriteToken(token);
        }

      public int? ValidateToken(string token)
 {
         if (string.IsNullOrEmpty(token))
        return null;

  var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration["Jwt:SecretKey"]
      ?? throw new Exception("JWT SecretKey tanýmlý deðil.");
            var key = Encoding.UTF8.GetBytes(secretKey);

       try
      {
      tokenHandler.ValidateToken(token, new TokenValidationParameters
   {
    ValidateIssuerSigningKey = true,
   IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
          ValidIssuer = _configuration["Jwt:Issuer"],
           ValidateAudience = true,
        ValidAudience = _configuration["Jwt:Audience"],
      ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
       }, out SecurityToken validatedToken);

         var jwtToken = (JwtSecurityToken)validatedToken;
   var userIdClaim = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

           return int.Parse(userIdClaim);
    }
            catch
    {
      return null;
            }
        }
    }
}
