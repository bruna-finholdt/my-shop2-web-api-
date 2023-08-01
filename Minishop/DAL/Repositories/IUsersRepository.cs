using Minishop.DAL.Base;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public interface IUsersRepository : IBaseRepository<User>
    {
        Task<List<User>> Pesquisar(int paginaAtual, int qtdPagina);
        Task<User> PesquisaPorId(int id);
        Task<User> PesquisaPorNome(string userName);

        Task<bool> VerificarUsernameExistente(string username);
        Task<User?> Cadastrar(User novo);
        Task<User> Editar(User model);

        Task<List<int>> GetValidRoleIds();

    }
}
