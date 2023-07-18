using Minishop.Domain.Entity;
using System.Collections.Generic;

namespace Minishop.Domain.DTO
{
    public class CustomerOrderCompletoResponse : CustomerOrderResponse
    {
        public CustomerOrderCompletoResponse(CustomerOrder customerOrder)
            : base(customerOrder)
        {
            OrderItems = customerOrder.OrderItems.Select(orderItem => new OrderItemResponse(orderItem)).ToList();

        }


        public List<OrderItemResponse> OrderItems { get; private set; }

    }

}
