using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class SuppliersRepository : BaseRepository<Supplier>, ISuppliersRepository
    {
        public SuppliersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        public async Task<List<Supplier>> Pesquisar(int paginaAtual, int qtdPagina)
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<Supplier>()
                .Include(x => x.Products)
                .OrderBy(product => product.CompanyName)
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }

        public override async Task<Supplier?> PesquisaPorId(int id)
        {
            // select top 1 * from T where id = :id
            return await _minishop2023Context
                .Suppliers
                .Include(x => x.Products)
                //.ThenInclude(x => x.OrderItems)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> VerificarCnpjExistente(string cnpj)
        {
            // Verifica se há algum fornecedor com o CNPJ especificado no banco de dados
            return await _minishop2023Context.Suppliers.AnyAsync(s => s.Cnpj == cnpj);
        }

        public async Task<bool> VerificarEmailExistente(string email)
        {
            // Verifica se há algum fornecedor com o e-mail especificado no banco de dados
            return await _minishop2023Context.Suppliers.AnyAsync(s => s.Email == email);
        }

        public async Task<bool> VerificarEmailExistente2(string email, int id)
        {
            return await _minishop2023Context.Suppliers.AnyAsync(s => s.Id != id && s.Email == email);
        }
    }
}
