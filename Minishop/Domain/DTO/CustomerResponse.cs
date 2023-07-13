using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class CustomerResponse
    {
        public CustomerResponse(Customer customer)
        {
            Id = customer.Id;
            Name = $"{customer.FirstName} {customer.LastName}";
            Phone = customer.Phone;
            Email = customer.Email;
        }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string? Phone { get; private set; }
        public string Email { get; private set; }
    }
}
