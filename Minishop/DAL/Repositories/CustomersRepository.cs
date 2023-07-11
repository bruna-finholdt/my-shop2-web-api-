using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class CustomersRepository : BaseRepository<Customer>
    {
        public CustomersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        public async Task<List<Customer>> Pesquisar(int paginaAtual, int qtdPagina)
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<Customer>()
                .OrderBy(customer => customer.FirstName)
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }
    }
}
