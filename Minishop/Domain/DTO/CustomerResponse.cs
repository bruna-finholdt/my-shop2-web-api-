using Minishop.Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class CustomerResponse
    {
        public CustomerResponse(Customer customer)
        {
            Id = customer.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Phone = customer.Phone;
            Email = customer.Email;
        }
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? Phone { get; private set; }
        public string Email { get; private set; }
    }
}
