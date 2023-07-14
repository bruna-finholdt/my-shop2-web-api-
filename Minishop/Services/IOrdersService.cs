using Minishop.Domain.DTO;
using Minishop.Services.Base;
using System.Threading.Tasks;

namespace Minishop.Services
{
    public interface IOrdersService
    {
        Task<int> Contar();
        Task<ServicePagedResponse<CustomerOrderResponse>> Pesquisar(PageQueryRequest queryRequest);
        Task<ServiceResponse<CustomerOrderCompletoResponse>> PesquisaPorId(int id);
    }
}
