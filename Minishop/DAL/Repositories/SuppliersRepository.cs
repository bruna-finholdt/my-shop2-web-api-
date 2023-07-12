using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class SuppliersRepository : BaseRepository<Supplier>
    {
        public SuppliersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        public async Task<List<Supplier>> Pesquisar(int paginaAtual, int qtdPagina)
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<Supplier>()
                .OrderBy(product => product.CompanyName)
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }
    }
}
