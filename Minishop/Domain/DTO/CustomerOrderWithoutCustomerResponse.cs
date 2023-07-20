using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class CustomerOrderWithoutCustomerResponse
    {
        public CustomerOrderWithoutCustomerResponse(CustomerOrder customerOrder)
        {
            Id = customerOrder.Id;
            OrderDate = customerOrder.OrderDate;
            TotalAmount = customerOrder.TotalAmount;
            QuantityItems = customerOrder.OrderItems.Count;
        }

        public int Id { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public int QuantityItems { get; private set; }
    }
}

