using System.ComponentModel.DataAnnotations.Schema;

namespace Minishop.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int Sequencia { get; set; }
        public Product Produto { get; set; }
        public int ProductId { get; set; } //FK p Product
    }
}
