using Minishop.Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class ProductCreateRequest
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de caracteres no nome do produto é 100")]
        public string ProductName { get; set; } = null!;
        [Required]
        public int SupplierId { get; set; }
        [Required(ErrorMessage = "O preço do produto é obrigatório.")]
        public decimal? UnitPrice { get; set; }
        [Required]
        public bool IsDiscontinued { get; set; }

        [StringLength(100, ErrorMessage = "Tamanho máximo de caracteres no nome do produto é 100")]
        public string? PackageName { get; set; }
    }
}
