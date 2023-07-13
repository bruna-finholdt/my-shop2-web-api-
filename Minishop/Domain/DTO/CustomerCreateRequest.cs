using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class CustomerCreateRequest
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "O sobrenome do cliente é obrigatório")]
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        [Required]
        public string? Cpf { get; set; }
    }
}
