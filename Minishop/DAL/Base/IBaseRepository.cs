using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services.Base;

namespace Minishop.DAL.Base
{
    public interface IBaseRepository<T> where T : class
    {
        Task<int> Contagem();
        //Task<T?> PesquisaPorId(int id);
        Task<T> Cadastrar(T novo);
        Task<T> Editar(T model);
    }
}
