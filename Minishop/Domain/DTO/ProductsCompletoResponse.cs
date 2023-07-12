using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class ProductsCompletoResponse : ProductsResponse
    {
        public ProductsCompletoResponse(Product product, Supplier supplier)
           : base(product)
        {
            Supplier = new SuppliersResponse(supplier);
        }

        public SuppliersResponse Supplier { get; private set; }
    }
}
