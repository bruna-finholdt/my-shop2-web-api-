using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class SupplierCreateRequest
    {
        [Required(ErrorMessage = "O nome do fornecedor é obrigatório")]
        public string CompanyName { get; set; } = null!;
        [Required]
        public string? Cnpj { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Uf { get; set; }
        public string? Phone { get; set; }
        public string? ContactName { get; set; }
    }
}
