using Minishop.Domain.DTO.Validation;
using System.ComponentModel.DataAnnotations;


namespace Minishop.Domain.DTO
{
    public class CustomerCreateRequest
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "O sobrenome do cliente é obrigatório")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "O e-mail do cliente é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }
        public string Phone { get; set; }

        [Required(ErrorMessage = "O CPF do cliente é obrigatório")]
        [Cpf(ErrorMessage = "CPF inválido")]
        //[StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter 11 dígitos")]
        public string Cpf { get; set; }
    }
}
