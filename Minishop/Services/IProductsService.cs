using Minishop.DAL.Base;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public interface IProductsService
    {
        Task<ProductCountResponse> Contar();
        Task<ServicePagedResponse<ProductsResponse>> Pesquisar(PageQueryRequest queryResquest);
        Task<ServiceResponse<ProductsCompletoResponse>> PesquisaPorId(int id);
        Task<ServiceResponse<ProductsResponse>> Cadastrar(ProductCreateRequest novo);
        Task<ServiceResponse<ProductsResponse>> Editar(int id, ProductUpdateRequest model);
    }
}
