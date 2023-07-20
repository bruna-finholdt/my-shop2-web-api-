using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Minishop.Domain.DTO.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class Cpf : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || !(value is string cpf))
            {
                return false;
            }

            // Remover caracteres não numéricos
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            // Verificar se o CPF possui 11 dígitos
            if (cpf.Length != 11)
            {
                return false;
            }

            // Verificar se todos os dígitos são iguais (CPF inválido)
            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }

            // Calcular os dígitos verificadores
            int[] numbers = cpf.Take(9).Select(c => c - '0').ToArray();
            int[] verificationDigits = new int[2];

            for (int i = 0; i < 2; i++)
            {
                int sum = numbers.Zip(Enumerable.Range(2, 9).Reverse(), (n, w) => n * w).Sum();
                int remainder = (sum * 10) % 11;
                verificationDigits[i] = remainder == 10 ? 0 : remainder;
                numbers = numbers.Append(verificationDigits[i]).ToArray();
            }

            // Verificar se os dígitos verificadores são válidos
            return cpf.EndsWith($"{verificationDigits[0]}{verificationDigits[1]}");
        }
    }
}
