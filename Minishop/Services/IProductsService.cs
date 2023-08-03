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
        Task<ServiceResponse<ProductsCompletoResponse>> Cadastrar(ProductCreateRequest novo);
        Task<ServiceResponse<ProductsCompletoResponse>> Editar(int id, ProductUpdateRequest model);
        Task<ServiceResponse<List<ProductImageResponse>>> RemoverImagem(int productId, ProductImageDeleteRequest model);
        Task<ServiceResponse<ProductImageResponse>> CadastrarImagem(IFormFile file, int productId);
        Task<ServiceResponse<List<ProductImageResponse>>> AlterarOrdemImagens(int productId, ProductImageOrderUpdateRequest model);
        Task<ServiceResponse<ProductsCompletoResponse>> PesquisaPorIdCompleto(int id);


    }
}
