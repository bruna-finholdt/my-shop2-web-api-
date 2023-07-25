using Minishop.Domain.DTO;
using Minishop.Services.Base;


namespace Minishop.Services
{
    public interface IUsersService
    {
        Task<ServicePagedResponse<UserResponse>> Pesquisar(PageQueryRequest queryResquest);
        Task<ServiceResponse<UserResponse>> PesquisaPorId(int id);
        Task<ServiceResponse<UserResponse>> Cadastrar(UserCreateRequest novo);
        Task<ServiceResponse<UserResponse>> Editar(int id, UserUpdateRequest model);
        Task<ServiceResponse<UserResponse>> PesquisaPorNome(string userName);
        Task<ServiceResponse<UserResponse>> Logar(UserLoginRequest model);
    }
}
