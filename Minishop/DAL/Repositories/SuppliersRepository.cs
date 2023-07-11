using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class SuppliersRepository : BaseRepository<Customer>
    {
        public SuppliersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }
    }
}
