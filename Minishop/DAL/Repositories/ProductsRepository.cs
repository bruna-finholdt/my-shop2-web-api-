using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;
using Minishop.Services;

namespace Minishop.DAL.Repositories
{
    public class ProductsRepository : BaseRepository<Product>, IProductsRepository
    {
        private readonly IStorageService _storageService;
        public ProductsRepository(Minishop2023Context minishop2023Context, IStorageService storageService) : base(minishop2023Context)
        {
            _storageService = storageService;
        }

        public async Task<int> ContagemProdutosAtivos()
        {
            return await _minishop2023Context.Products.CountAsync(x => !x.IsDiscontinued);
        }

        public async Task<int> ContagemProdutosInativos()
        {
            return await _minishop2023Context.Products.CountAsync(x => x.IsDiscontinued);
        }

        public async Task<List<Product>> Pesquisar(int paginaAtual, int qtdPagina)
        {
            int qtaPaginasAnteriores = paginaAtual * qtdPagina - qtdPagina;

            return await _minishop2023Context
                .Set<Product>()
                .OrderBy(product => product.ProductName)
                .Skip(qtaPaginasAnteriores)
                .Take(qtdPagina)
                .ToListAsync();
        }

        public override async Task<Product?> PesquisaPorId(int id)
        {
            // select top 1 * from T where id = :id
            return await _minishop2023Context
                .Products
                .Include(x => x.Supplier)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<ProductImage> CadastrarImagem(ProductImage productImage)
        {
            // Verifica se o productId existe no banco de dados
            var existingProduct = await _minishop2023Context.Products.FindAsync(productImage.ProductId);
            if (existingProduct == null)
            {
                return null; // Produto não encontrado, retorna null
            }

            // Produto encontrado, cadastra a imagem
            _minishop2023Context.ProductImages.Add(productImage);
            await _minishop2023Context.SaveChangesAsync();

            return productImage;
        }

        public async Task<bool> RemoverImagem(int imageId)
        {
            var productImage = await _minishop2023Context.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                return false;//Imagem não encontrada
            }

            // Remove a imagem do bucket
            var imageUrlParts = productImage.Url.Split('/');
            var key = imageUrlParts.Last();
            await _storageService.RemoveImageFromBucket(key);

            _minishop2023Context.ProductImages.Remove(productImage);
            await _minishop2023Context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetImageCount(int productId)
        {
            return await _minishop2023Context.ProductImages.CountAsync(p => p.ProductId == productId);
        }

        public async Task<bool> ReorganizarSequenciaDeImagens(int productId)
        {
            //Obtém todas as imagens associadas aquele produto e as ordena pelo seu valor de sequencia
            var images = await _minishop2023Context.ProductImages
                .Where(image => image.ProductId == productId)
                .OrderBy(image => image.Sequencia)
                .ToListAsync();

            //Atualiza a sequencia para cada imagem de acordo com o seu indice
            for (int i = 0; i < images.Count; i++)
            {
                images[i].Sequencia = i + 1;
            }
            try
            {
                await _minishop2023Context.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public async Task<ProductImage> GetImageById(int imageId)
        {
            return await _minishop2023Context.ProductImages.FindAsync(imageId);
        }

        public async Task<List<ProductImage>> GetImagesByProductId(int productId)
        {
            //Recupera todas as imagens associadas aquele produto no db
            return await _minishop2023Context.ProductImages
                .Where(img => img.ProductId == productId)
                .ToListAsync();
        }

        public async Task EditarImagem(ProductImage productImage)
        {
            _minishop2023Context.Entry(productImage).State = EntityState.Modified;
            await _minishop2023Context.SaveChangesAsync();
        }

        public async Task<int> GetHighestSequence(int productId)
        {
            var highestSequence = await _minishop2023Context.ProductImages
           .Where(p => p.ProductId == productId)
           .OrderByDescending(p => p.Sequencia)
           .Select(p => p.Sequencia)
           .FirstOrDefaultAsync();

            return highestSequence;
        }
    }
}
