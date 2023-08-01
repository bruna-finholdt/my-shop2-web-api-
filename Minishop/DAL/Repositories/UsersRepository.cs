using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using System.Text;

namespace Minishop.DAL.Repositories
{
    public class UsersRepository : BaseRepository<User>, IUsersRepository
    {
        public UsersRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
        }

        public async Task<List<User>> Pesquisar(int paginaAtual, int qtdPagina)
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<User>()
                .Include(x => x.Role)
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }

        public async Task<List<int>> GetValidRoleIds()
        {
            return await _minishop2023Context.Roles.Select(role => role.Id).ToListAsync();
        }

        public override async Task<User?> PesquisaPorId(int id)
        {
            // select top 1 * from T where id = :id
            return await _minishop2023Context
                .Users
                .Include(x => x.Role)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> PesquisaPorNome(string userName)
        {
            return await _minishop2023Context.Users.Include(x => x.Role).FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> VerificarUsernameExistente(string username)
        {
            return await _minishop2023Context.Users.AnyAsync(x => x.UserName == username);
        }

        public override async Task<User?> Cadastrar(User novo)
        {
            _minishop2023Context.Add(novo);

            novo.Role = await _minishop2023Context.Roles.FindAsync(novo.RoleId);

            await _minishop2023Context.SaveChangesAsync();
            return novo;
        }

        public override async Task<User> Editar(User model)
        {
            _minishop2023Context.Entry(model).State = EntityState.Modified;

            model.Role = await _minishop2023Context.Roles.FindAsync(model.RoleId);

            await _minishop2023Context.SaveChangesAsync();
            return model;
        }

    }
}
