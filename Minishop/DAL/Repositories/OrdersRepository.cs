using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class OrdersRepository : BaseRepository<OrderItem>
    {
        public OrdersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        ///Contagem de pedidos
        ///<returns>Quantidade de pedidos</returns>
        public async Task<int> ContagemPedidos()
        {
            return await _minishop2023Context.OrderItems.Select(x => x.OrderId).Distinct().CountAsync();
        }
    }
}
