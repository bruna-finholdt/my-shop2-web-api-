using System.Collections.Generic;
using System.Threading.Tasks;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public interface IProductsRepository : IBaseRepository<Product>
    {
        Task<int> ContagemProdutosAtivos();
        Task<int> ContagemProdutosInativos();
        Task<List<Product>> Pesquisar(int paginaAtual, int qtdPagina);
        //Task<List<Product>> PesquisarSupplierId(int supplierId);
        Task<Product> PesquisaPorId(int id);
        //Task<bool> VerificarFornecedorExistente(int id);
        Task<bool> VerificarFornecedorExistente(int? supplierId);
    }
}

