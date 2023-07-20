using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Minishop.Domain.DTO.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class Cpf : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("O CPF do cliente é obrigatório");
            }

            var cpf = value.ToString().Replace(".", "").Replace("-", "");

            if (!IsCpfValid(cpf))
            {
                return new ValidationResult("CPF inválido");
            }

            return ValidationResult.Success;
        }


        private static bool IsCpfValid(string cpf)
        {
            if (cpf.Length != 11)
            {
                return false;
            }


            if (cpf == "00000000000" || cpf == "11111111111" || cpf == "22222222222" || cpf == "33333333333"
                || cpf == "44444444444" || cpf == "55555555555" || cpf == "66666666666" || cpf == "77777777777"
                || cpf == "88888888888" || cpf == "99999999999")
            {
                return false;
            }

            int[] numbers = new int[11];

            for (int i = 0; i < 11; i++)
            {
                numbers[i] = int.Parse(cpf[i].ToString());
            }

            int sum1 = 0;
            for (int i = 0; i < 9; i++)
            {
                sum1 += numbers[i] * (10 - i);
            }

            int remainder1 = sum1 % 11;
            int digit1 = remainder1 < 2 ? 0 : 11 - remainder1;

            if (digit1 != numbers[9])
            {
                return false;
            }

            int sum2 = 0;
            for (int i = 0; i < 10; i++)
            {
                sum2 += numbers[i] * (11 - i);
            }

            int remainder2 = sum2 % 11;
            int digit2 = remainder2 < 2 ? 0 : 11 - remainder2;

            return digit2 == numbers[10];
        }
    }
}
