using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class CustomersRepository : BaseRepository<Customer>, ICustomersRepository
    {
        public CustomersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        public async Task<List<Customer>> Pesquisar(int paginaAtual, int qtdPagina)
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<Customer>()
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }

        public override async Task<Customer?> PesquisaPorId(int id)
        {
            // select top 1 * from T where id = :id
            return await _minishop2023Context
                .Customers
                .Include(x => x.CustomerOrders)
                    .ThenInclude(x => x.OrderItems)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> VerificarCpfExistente(string cpf)
        {
            return await _minishop2023Context.Customers.AnyAsync(x => x.Cpf == cpf);
        }

        public async Task<bool> VerificarEmailExistente(string email)
        {
            return await _minishop2023Context.Customers.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> VerificarNovoEmailExistente(string email, int id)
        {
            // Verificar se o e-mail já está em uso por outro cliente (exceto o cliente com o ID fornecido)
            return await _minishop2023Context.Customers.AnyAsync(c => c.Email == email && c.Id != id);
        }
    }
}
