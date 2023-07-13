using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Minishop.DAL.Base
{
    /// <summary>
    /// Essa classe tem o poder de realizar consultas simples em todas as tabelas
    /// </summary>
    /// <typeparam name="T">Entidade da aplicação</typeparam>
    public class BaseRepository<T> where T : class
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

        ///// <summary>
        ///// Consulta por id
        ///// </summary>
        ///// <remarks>
        ///// Se o metodo é async sempre devemos retornar uma Task
        ///// </remarks>
        ///// <see cref="https://docs.microsoft.com/dotnet/csharp/programming-guide/concepts/async/"/>
        ///// <param name="id">número inteiro do id da entidade</param>
        ///// <returns>Customer do id consultado ou não encontrado</returns>
        public virtual async Task<T?> PesquisaPorId(int id)//Para usar um método async, devemos colocar
                                                           //async na assinatura, Task<> no retorno e
                                                           //colocar o await na chamada de qualquer método
                                                           //async interno.
        {
            // select top 1 * from T where id = :id
            return await _minishop2023Context.Set<T>().FindAsync(id); //Para acessar a entidade usamos o método
                                                                      //Set< T>() ele nos dá a possibilidade de
                                                                      //acessar a tabela da entidade T, que assumirá
                                                                      //o valor de cada uma das entidades 
        }


        ///// <summary>
        ///// Cadastra entidade enviada
        ///// </summary>
        ///// <param name="novo">Nova entidade</param>
        ///// <returns>Entidade criada</returns>
        public async Task<T> Cadastrar(T novo)
        {
            _minishop2023Context.Add(novo);
            await _minishop2023Context.SaveChangesAsync(); // Todo o Entity está preparado para isso
            return novo;
        }

        ///// <summary>
        ///// Cadastra entidades enviadas
        ///// </summary>
        ///// <param name="novo">Novas entidades</param>
        ///// <returns>Entidades criadas</returns>
        //public async Task<IEnumerable<T>> CadastrarVarios(IEnumerable<T> novos)
        //{
        //    _minishop2023Context.AddRange(novos);
        //    await _minishop2023Context.SaveChangesAsync(); // Todo o Entiy está preparado para isso
        //    return novos;
        //}

        ///// <summary>
        ///// Realiza contagem de quantas linhas existem naquela tabela
        ///// Necessário para formar paginação
        ///// </summary>
        ///// <returns>Quantidade de itens naquela tabela</returns>
        //public async Task<int> Contagem() //Realiza contagem de quantos T existem no banco
        //{
        //    return await _minishop2023Context.Set<T>().CountAsync();
        //}

        ///// <summary>
        ///// Realiza contagem de quantas linhas existem que atendem o filtro na tabela
        ///// Necessário para formar paginação
        ///// </summary>
        ///// <param name="filtro">
        ///// Expressão Lambda que filtra a consulta de acordo com a condição booeana
        ///// </param>
        ///// <returns>Quantidade de itens que atendem o filtro na tabela</returns>
        //public async Task<int> Contagem(Expression<Func<T, bool>> filtro)
        //{
        //    return await _minishop2023Context.Set<T>().CountAsync(filtro);
        //}

        /// <summary>
        /// Lista Entidade com paginação
        /// </summary>
        /// <param name="paginaAtual">Número da atual página de 0 até N</param>
        /// <param name="qtdPagina">Número de itens por página de 1 até 50</param>
        /// <returns>Lista de T com informações de paginação</returns>
        //public async Task<List<T>> Pesquisar(int paginaAtual, int qtdPagina) //Lista de T com paginação
        //{
        //    // Estou na página 4 (começando em 0), e tem 20 itens por página
        //    // descarto os primeiro 80, pego os próximos 20
        //    //(80 = 20 da pagina 0 + 20 da pagina 1 + 20 da pagina 2 + 20 da pag 3. Pego os proxs 20
        //    //da pag 4, a que estou!)
        //    int qtaPaginasAnteriores = (paginaAtual - 1) * qtdPagina;

        //    return await _minishop2023Context
        //        .Set<T>()
        //        .Skip(qtaPaginasAnteriores)
        //        .Take(qtdPagina)
        //        .ToListAsync();
        ////}

        //public async Task<List<T>> Pesquisar() //Lista de T com paginação
        //{
        //    // Estou na página 4 (começando em 0), e tem 20 itens por página
        //    // descarto os primeiro 80, pego os próximos 20
        //    //(80 = 20 da pagina 0 + 20 da pagina 1 + 20 da pagina 2 + 20 da pag 3. Pego os proxs 20
        //    //da pag 4, a que estou!)

        //    return await _minishop2023Context
        //        .Set<T>()
        //        .ToListAsync();
        //}

        ///// <summary>
        ///// Lista itens da Entidade que atendem o filtro na tabela com paginação
        ///// </summary>
        ///// <param name="filtro">
        ///// Expressão Lambda que filtra a consulta de acordo com a condição booeana
        ///// </param>
        ///// <param name="paginaAtual">Número da atual página de 0 até N</param>
        ///// <param name="qtdPagina">Número de itens por página de 1 até 50</param>
        ///// <returns>Lista filtrada de T com informações de paginação</returns>
        //public async Task<List<T>> Pesquisar(Expression<Func<T, bool>> filtro, int paginaAtual, int qtdPagina)//Lista de T com paginação e filtros
        //{
        //    // Estou na página 4 (começando em 0), e tem 20 itens por página
        //    // descarto os primeiro 80, pego os próximos 20
        //    //(80 = 20 da pagina 0 + 20 da pagina 1 + 20 da pagina 2 + 20 da pag 3. Pego os proxs 20
        //    //da pag 4, a que estou!)
        //    int qtaPaginasAnteriores = paginaAtual * qtdPagina;

        //    return await _minishop2023Context
        //        .Set<T>()
        //        .Where(filtro)
        //        .Skip(qtaPaginasAnteriores)
        //        .Take(qtdPagina)
        //        .ToListAsync();
        //}
    }
}
