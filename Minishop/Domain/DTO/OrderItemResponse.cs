using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class OrderItemResponse
    {
        public OrderItemResponse(OrderItem orderItem)
        {
            Id = orderItem.Id;
            OrderId = orderItem.OrderId;
            ProductId = orderItem.ProductId;
            Quantity = orderItem.Quantity;
            UnitPrice = orderItem.UnitPrice;
            //Product = orderItem.Product;
            //CustomerOrder = orderItem.

        }

        public int Id { get; private set; }

        public int OrderId { get; private set; }

        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        //public Product Product { get; private set; }
        //public CustomerOrder CustomerOrder { get; private set; }
    }
}
