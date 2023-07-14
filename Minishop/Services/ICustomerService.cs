using System.Threading.Tasks;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public interface ICustomersService
    {
        Task<int> Contar();
        Task<ServicePagedResponse<CustomerResponse>> Pesquisar(PageQueryRequest queryResquest);
        Task<ServiceResponse<CustomerCompletoResponse>> PesquisaPorId(int id);
        Task<ServiceResponse<CustomerResponse>> Cadastrar(CustomerCreateRequest novo);
        Task<ServiceResponse<Customer>> Editar(int id, CustomerUpdateRequest model);
    }
}
