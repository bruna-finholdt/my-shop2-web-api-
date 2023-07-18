using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class SuppliersResponse
    {
        public SuppliersResponse(Supplier supplier)
        {
            if (supplier != null)
            {
                Id = supplier.Id;
                CompanyName = supplier.CompanyName;
                ContactName = supplier.ContactName;
                Phone = supplier.Phone;
                City = supplier.City;
                Uf = supplier.Uf;
                Email = supplier.Email;
            }
        }
        public int Id { get; private set; }
        public string CompanyName { get; private set; }
        public string? ContactName { get; private set; }
        public string? Phone { get; private set; }
        public string City { get; private set; }
        public string Uf { get; private set; }
        public string Email { get; private set; }
    }
}
