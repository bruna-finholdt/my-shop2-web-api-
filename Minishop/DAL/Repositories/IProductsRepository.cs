using System.Collections.Generic;
using System.Threading.Tasks;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public interface IProductsRepository : IBaseRepository<Product>
    {
        Task<int> ContagemProdutosAtivos();
        Task<int> ContagemProdutosInativos();
        Task<List<Product>> Pesquisar(int paginaAtual, int qtdPagina);
        //Task<List<Product>> PesquisarSupplierId(int supplierId);
        Task<Product> PesquisaPorId(int id);
        Task<ProductImage> CadastrarImagem(ProductImage productImage);
        Task<bool> RemoverImagem(int imageId);
        Task<int> GetImageCount(int productId);
        Task<bool> ReorganizarSequenciaDeImagens(int productId);
        Task<List<ProductImage>> GetImagesByProductId(int productId);
        //Task<bool> AtualizarSequenciaDeImagens(List<ProductImage> images);
        Task EditarImagem(ProductImage productImage);
        Task<int> GetHighestSequence(int productId);




    }
}

