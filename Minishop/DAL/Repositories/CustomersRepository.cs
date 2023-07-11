using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class CustomersRepository : BaseRepository<Customer>
    {
        public CustomersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

    }
}
