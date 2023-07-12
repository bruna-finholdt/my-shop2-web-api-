using Minishop.Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class ProductsResponse
    {
        public ProductsResponse(Product product)
        {
            Id = product.Id;
            ProductName = product.ProductName;
            SupplierId = product.SupplierId;
            UnitPrice = product.UnitPrice;
            IsDiscontinued = product.IsDiscontinued;
            PackageName = product.PackageName;
        }

        public int Id { get; private set; }
        public string ProductName { get; private set; }
        public int SupplierId { get; private set; }
        public decimal? UnitPrice { get; private set; }
        public bool IsDiscontinued { get; private set; }
        public string? PackageName { get; private set; }
    }
}
