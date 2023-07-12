using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class CustomerOrderResponse
    {
        public CustomerOrderResponse(CustomerOrder customerOrder)
        {
            Id = customerOrder.Id;
            OrderDate = customerOrder.OrderDate;
            TotalAmount = customerOrder.TotalAmount;
            Customer = new CustomerResponse(customerOrder.Customer);
            QuantityItems = customerOrder.OrderItems.Count;
        }
        public int Id { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public CustomerResponse Customer { get; private set; }
        public int QuantityItems { get; private set; }
    }
}
