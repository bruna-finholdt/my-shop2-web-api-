namespace Minishop.Domain.DTO
{
    public class ProductAndImageResponse
    {
        public ProductAndImageResponse(ProductsCompletoResponse productResponse, List<ProductImageResponse> images)
        {
            ProductResponse = productResponse;
            Images = images;
        }

        public ProductsCompletoResponse ProductResponse { get; set; }
        public List<ProductImageResponse> Images { get; set; }
    }
}

