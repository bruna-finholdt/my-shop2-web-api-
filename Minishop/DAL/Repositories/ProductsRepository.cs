using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class ProductsRepository : BaseRepository<Product>
    {
        public ProductsRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        public async Task<int> ContagemProdutosAtivos()
        {
            return await _minishop2023Context.Products.CountAsync(x => !x.IsDiscontinued);
        }

        public async Task<int> ContagemProdutosInativos()
        {
            return await _minishop2023Context.Products.CountAsync(x => x.IsDiscontinued);
        }

        public async Task<List<Product>> Pesquisar(int paginaAtual, int qtdPagina) 
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<Product>()
                .OrderBy(product => product.ProductName)
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }


    }
}
