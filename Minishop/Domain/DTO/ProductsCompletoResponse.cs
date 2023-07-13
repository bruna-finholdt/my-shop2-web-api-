using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class ProductsCompletoResponse : ProductsResponse
    {
        public ProductsCompletoResponse(Product product)
           : base(product)
        {
            Supplier = new SuppliersResponse(product.Supplier);
        }

        public SuppliersResponse Supplier { get; private set; }
    }
}
