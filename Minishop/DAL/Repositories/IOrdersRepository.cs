using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public interface IOrdersRepository : IBaseRepository<CustomerOrder>
    {
        Task<List<CustomerOrder>> Pesquisar(int paginaAtual, int qtdPagina);
        Task<CustomerOrder> PesquisarPorId(int id);
    }
}
