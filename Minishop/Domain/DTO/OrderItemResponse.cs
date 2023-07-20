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
            ProductName = orderItem.Product?.ProductName;
            TotalAmount = orderItem.UnitPrice * Quantity;

        }

        public int Id { get; private set; }

        public int OrderId { get; private set; }

        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public string ProductName { get; private set; }
        public decimal TotalAmount { get; private set; }

    }
}
