using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public interface ICustomersRepository : IBaseRepository<Customer>
    {
        Task<List<Customer>> Pesquisar(int paginaAtual, int qtdPagina);
        Task<Customer> PesquisaPorId(int id);
        Task<bool> VerificarCpfExistente(string cpf);
        Task<bool> VerificarEmailExistente(string email);
        Task<bool> VerificarNovoEmailExistente(string email, int id);
    }
}
