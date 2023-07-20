using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Minishop.Domain.DTO.Validation
{
    public class Email : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("O e-mail do cliente é obrigatório");
            }

            var email = value.ToString();

            if (!IsValidEmail(email))
            {
                return new ValidationResult("E-mail inválido");
            }

            return ValidationResult.Success;
        }


        private static bool IsValidEmail(string email)
        {

            const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}
