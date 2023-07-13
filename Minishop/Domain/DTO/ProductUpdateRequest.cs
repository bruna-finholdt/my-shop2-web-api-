using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class ProductUpdateRequest
    {
      
        [MinLength(10, ErrorMessage = "O tamanho mínimo de caracteres no nome do produto é 10")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de caracteres no nome do produto é 100")]
        public string ProductName { get; set; } = null!;
      
       
        public decimal? UnitPrice { get; set; }
       
        public bool IsDiscontinued { get; set; }

        [StringLength(100, ErrorMessage = "Tamanho máximo de caracteres no nome do produto é 100")]
        public string? PackageName { get; set; }
    }
}
