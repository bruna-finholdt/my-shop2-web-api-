using Microsoft.IdentityModel.Tokens;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Minishop.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken(UserResponse user)
        {
            if (user == null)
            {
                // Tratar o cenário de usuário nulo, lançar uma exceção personalizada.
                throw new ArgumentNullException(nameof(user), "Usuário não encontrado ou nulo.");
            }
            else
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Settings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(8),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), algorithm: SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }

            
        }
    }
}
