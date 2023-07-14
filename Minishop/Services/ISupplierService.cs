using System.Threading.Tasks;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public interface ISupplierService
    {
        Task<int> Contar();
        Task<ServicePagedResponse<SuppliersResponse>> Pesquisar(PageQueryRequest queryRequest);
        Task<ServiceResponse<SuppliersCompletoResponse>> PesquisaPorId(int id);
        Task<ServiceResponse<SuppliersResponse>> Cadastrar(SupplierCreateRequest novo);
        Task<ServiceResponse<Supplier>> Editar(int id, SupplierUpdateRequest model);
    }
}
