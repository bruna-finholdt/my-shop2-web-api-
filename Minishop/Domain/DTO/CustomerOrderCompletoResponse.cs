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

            //OrderItems = new List<OrderItemResponse>();

            //foreach (OrderItem orderItem in orderItems)
            //{

            //    //var itemResponse = new OrderItemResponse(orderItem);
            //    //OrderItems.Add(itemResponse);
            //    OrderItems.Add(new OrderItemResponse(orderItem));
            //}

        }


        public List<OrderItemResponse> OrderItems { get; private set; }

    }

}
