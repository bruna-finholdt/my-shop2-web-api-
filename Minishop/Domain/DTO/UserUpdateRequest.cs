using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class UserUpdateRequest
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        [Range(1, 3, ErrorMessage = "RoleId inválido! O RoleId do usuário deve ser 1 para 'admin', 2 para 'common' ou 3 para 'seller'.")]
        public int ? RoleId { get; set; }
    }
}
