using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class SupplierUpdateRequest
    {
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Uf { get; set; }
        public string? Phone { get; set; }
        public string? ContactName { get; set; }
    }
}
