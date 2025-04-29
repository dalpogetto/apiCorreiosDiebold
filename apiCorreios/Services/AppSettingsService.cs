using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using apiCorreios.Models;
using Microsoft.IdentityModel.Tokens;

namespace apiCorreios.Services
{
    public static class AppSettingsService
    {
        public static JwtSettings JwtSettings { get; }

        static AppSettingsService()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            JwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
        }
    }

    public static class JwtBearerService
    {
        public static string GenerateToken(Usuario user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppSettingsService.JwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.Nome),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                ]),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

}
