using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class ProductsCompletoResponse : ProductsResponse
    {
        public ProductsCompletoResponse(Product product)
           : base(product)
        {
            Supplier = new SuppliersResponse(product.Supplier);
            Images = new List<ProductImageResponse>(); // Inicializa a lista de imagens
        }

        public SuppliersResponse Supplier { get; private set; }
        public List<ProductImageResponse> Images { get; set; } // Lista das imagens associadas ao produto

    }
}
