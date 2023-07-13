using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class OrderItemsRepository : BaseRepository<OrderItem>
    {
        public OrderItemsRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        public async Task<List<OrderItem>> PesquisarCustomerOrderId(int customerOrderId)
        {
            return await _minishop2023Context
                    .Set<OrderItem>()
                    .Include(x => x.Order)
                    .Where(x => x.OrderId == customerOrderId)
                    .ToListAsync();
        }
    }
}
