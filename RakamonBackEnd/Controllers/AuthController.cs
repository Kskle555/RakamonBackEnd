using DataAccess.DB;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RakamonBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _jwtSecretKey; // Secret key'i güvenli bir şekilde tutmak için

        public AuthController(AppDbContext context)
        {
            _context = context;
            _jwtSecretKey = GenerateSecretKey(); // Secret key'i burada oluşturuyoruz
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var users = _context.Set<User>().ToList();

                var user = users.FirstOrDefault(u => u.TcKimlikNo.ToString() == request.TcKimlikNo && u.Password.ToString() == request.Password);

                if (user == null)
                {
                    return Unauthorized(new { message = "TC Kimlik No veya Şifre hatalı" });
                }

                // Kullanıcı doğrulandıktan sonra JWT token oluşturuluyor
                var token = GenerateJwtToken(user);

                // Kullanıcı bilgileri ve token ile birlikte başarılı giriş mesajı dönülüyor
                return Ok(new
                {
                    message = "Giriş başarılı",
                    user = user,
                    token = token
                });
            }
            catch (Exception ex)
            {
                // Hata log'lama
                Console.WriteLine($"Login sırasında hata: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Çıkış başarılı" });
        }

        // JWT oluşturma methodu
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.TcKimlikNo.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey)); // Secret key'i kullanarak key oluşturuluyor
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your-app", // Uygulamanızın adı
                audience: "your-app", // Uygulamanızın hedef kitlesi
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Token'ın geçerlilik süresi (1 saat)
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Token'ı string formatında döndürüyoruz
        }

        // Secret key oluşturma methodu
        private string GenerateSecretKey()
        {
            // Güvenli bir anahtar oluşturmak için RNGCryptoServiceProvider kullanıyoruz
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[32]; // 256 bit key
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes); // Base64 string olarak geri döndürülüyor
            }
        }

        // Token doğrulama methodu
        private bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var key = Encoding.UTF8.GetBytes(_jwtSecretKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "your-app",
                    ValidAudience = "your-app",
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Token'ın süresi dolmuşsa hemen hata ver
                }, out var validatedToken);

                return true; // Token geçerli
            }
            catch
            {
                return false; // Token geçersiz
            }
        }

        public class LoginRequest
        {
            public string TcKimlikNo { get; set; }
            public string Password { get; set; }
        }
    }
}
