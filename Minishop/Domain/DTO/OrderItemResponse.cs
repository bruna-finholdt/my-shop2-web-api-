using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class OrderItemResponse
    {
        public OrderItemResponse(OrderItem orderItem)
        {
            Id = orderItem.Id;
            ProductName = orderItem.Product?.ProductName;
            Quantity = orderItem.Quantity;
            UnitPrice = orderItem.UnitPrice;
            TotalAmountItem = orderItem.UnitPrice * orderItem.Quantity;
        }

        public int Id { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalAmountItem { get; private set; }
    }
}
