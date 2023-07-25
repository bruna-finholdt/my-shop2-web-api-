using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Minishop.DAL.Base
{
    /// <summary>
    /// Essa classe tem o poder de realizar consultas simples em todas as tabelas
    /// </summary>
    /// <typeparam name="T">Entidade da aplicação</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        //usando o Minishop2023Context via injeção de dependência:
        protected readonly Minishop2023Context _minishop2023Context;

        public BaseRepository(Minishop2023Context context)
        {
            _minishop2023Context = context;
        }

        ///Contagem de itens
        /// <returns>Quantidade de itens naquela tabela</returns>
        public async Task<int> Contagem()
        {
            return await _minishop2023Context.Set<T>().CountAsync();
        }

        /// <summary>
        /// Consulta por id
        /// </summary>
        /// <remarks>
        /// Se o metodo é async sempre devemos retornar uma Task
        /// </remarks>
        /// <see cref="https://docs.microsoft.com/dotnet/csharp/programming-guide/concepts/async/"/>
        /// <param name="id">número inteiro do id da entidade</param>
        /// <returns>Customer do id consultado ou não encontrado</returns>
        public virtual async Task<T?> PesquisaPorId(int id)
        {
            // select top 1 * from T where id = :id
            return await _minishop2023Context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Cadastra entidade enviada
        /// </summary>
        /// <param name="novo">Nova entidade</param>
        /// <returns>Entidade criada</returns>
        public virtual async Task<T?> Cadastrar(T novo)
        {
            _minishop2023Context.Add(novo);
            await _minishop2023Context.SaveChangesAsync();
            return novo;
        }

        /// <summary>
        /// Edita entidade enviada
        /// </summary>
        /// <param name="model">Nova entidade</param>
        /// <returns>Entidade atualizada</returns>
        public virtual async Task<T> Editar(T model)
        {
            _minishop2023Context.Entry(model).State = EntityState.Modified;
            await _minishop2023Context.SaveChangesAsync();
            return model;
        }
    }
}
