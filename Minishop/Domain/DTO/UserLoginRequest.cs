using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "O username é obrigatório")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }
    }
}
