using Minishop.Domain.DTO;
using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class CustomerCompletoResponse : CustomerResponse
    {
        public CustomerCompletoResponse(Customer customer)
      : base(customer)
        {
            Orders = customer.CustomerOrders.Select(order => new CustomerOrderResponse(order)).ToList();
            TotalAmountSpent = customer.CustomerOrders.Sum(order => order.TotalAmount);
            Cpf = customer.Cpf;
        }


        public List<CustomerOrderResponse> Orders { get; private set; }
        public decimal TotalAmountSpent { get; private set; }
        public string Cpf { get; private set; }
    }
}



