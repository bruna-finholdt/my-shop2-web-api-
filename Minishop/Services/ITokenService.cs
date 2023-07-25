using Minishop.Domain.DTO;
using Minishop.Domain.Entity;

namespace Minishop.Services
{
    public interface ITokenService
    {
        string GenerateToken(UserResponse user);
    }
}
