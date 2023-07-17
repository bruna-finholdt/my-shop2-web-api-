using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public interface ISuppliersRepository : IBaseRepository<Supplier>
    {
        Task<List<Supplier>> Pesquisar(int paginaAtual, int qtdPagina);
        Task<Supplier> PesquisarPorId(int id);
    }
}
