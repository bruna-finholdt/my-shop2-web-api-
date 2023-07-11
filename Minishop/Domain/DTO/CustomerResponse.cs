using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class CustomerResponse
    {
        public CustomerResponse(Customer customer)
        {
            Id = customer.Id;
            FistName = customer.FirstName;
            LastName = customer.LastName;
            Phone = customer.Phone;
            Email = customer.Email;
        }
        public int Id { get; private set; }
        public string FistName { get; private set; }
        public string? LastName { get; private set; }
        public string? Phone { get; private set; }
        public string Email { get; private set; }
    }
}
