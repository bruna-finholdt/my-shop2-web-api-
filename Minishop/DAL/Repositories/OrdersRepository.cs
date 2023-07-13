using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class OrdersRepository : BaseRepository<CustomerOrder>
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

        public async Task<List<CustomerOrder>> Pesquisar(int paginaAtual, int qtdPagina)
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<CustomerOrder>()
                .Include(x => x.Customer)
                .Include(x => x.OrderItems)
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }

        //public async Task<List<CustomerOrder>> PesquisarCustomerId(int customerId)
        //{
        //    return await _minishop2023Context
        //            .Set<CustomerOrder>()
        //            .Include(x => x.Customer)
        //            .Include(x => x.OrderItems)
        //            .Where(x => x.CustomerId == customerId)
        //            .ToListAsync();
        //}

        public override async Task<CustomerOrder> PesquisaPorId(int id)
        {
            // select top 1 * from T where id = :id
            return await _minishop2023Context
                .CustomerOrders
                .Include(x => x.Customer)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Product)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

    }
}
