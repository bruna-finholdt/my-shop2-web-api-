//using System.ComponentModel.DataAnnotations;

//namespace Minishop.Domain.DTO
//{
//    public class ProductCreateRequest
//    {
//        [Required(ErrorMessage = "O nome do produto é obrigatório")]
//        [MinLength(10, ErrorMessage = "O tamanho mínimo de caracteres no nome do produto é 10")]
//        [StringLength(100, ErrorMessage = "Tamanho máximo de caracteres no nome do produto é 100")]
//        public string ProductName { get; set; } = null!;
//        public int SupplierId { get; private set; }
//        public decimal? UnitPrice { get; private set; }
//        public bool IsDiscontinued { get; private set; }
//        public string? PackageName { get; private set; }
//    }
//}
