using k8s.KubeConfigModels;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class ProductImageResponse
    {
        public ProductImageResponse(ProductImage productImage)
        {
            Id = productImage.Id;
            Url = productImage.Url;
            Sequencia = productImage.Sequencia;
            if (productImage.Produto != null)
            {
                ProductId = productImage.Produto.Id;

            };

        }

        public int Id { get; set; }
        public string Url { get; set; }
        public int? Sequencia { get; set; }
        public int ProductId { get; set; }

    }
}

