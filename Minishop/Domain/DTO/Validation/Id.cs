using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO.Validation
{
    public class Id : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            int teste;
            return value != null && value is string && int.TryParse((string)value, out teste);
        }
    }
}
