using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class UserCreateRequest
    {
        [Required(ErrorMessage = "O nome do usuário é obrigatório")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "O sobrenome do usuário é obrigatório")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A confirmação da senha é obrigatória")]
        public string PasswordConfirmation { get; set; }

        [Required(ErrorMessage = "O RoleId do usuário é obrigatório")]
        //[Range(1, 3, ErrorMessage = "RoleId inválido! O RoleId do usuário deve ser 1 para 'admin', 2 para 'common' ou 3 para 'seller'.")]
        public int RoleId { get; set; }
    }
}
