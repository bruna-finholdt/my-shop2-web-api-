using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public class ProductsRepository : BaseRepository<Product>, IProductsRepository
    {
        public ProductsRepository(Minishop2023Context minishop2023Context) : base(minishop2023Context)
        {
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

        //testar
        public async Task<ProductImage> CadastrarImagem(ProductImage productImage)
        {
            _minishop2023Context.ProductImages.Add(productImage);
            await _minishop2023Context.SaveChangesAsync();

            return productImage;
        }

        //testar
        public async Task<bool> RemoverImagem(int imageId)
        {
            var productImage = await _minishop2023Context.ProductImages.FindAsync(imageId);
            if (productImage == null)
            {
                return false;//Imagem não encontrada
            }

            _minishop2023Context.ProductImages.Remove(productImage);
            await _minishop2023Context.SaveChangesAsync();

            return true;
        }

        //testar
        public async Task<int> GetImageCount(int productId)
        {
            return await _minishop2023Context.ProductImages.CountAsync(p => p.ProductId == productId);
        }

        //testar
        public async Task<bool> ReorganizarSequenciaDeImagens(int productId)
        {
            // Get all images associated with the product and order them by sequence
            var images = await _minishop2023Context.ProductImages
                .Where(image => image.ProductId == productId)
                .OrderBy(image => image.Sequencia)
                .ToListAsync();

            // Update the sequence for each image based on its index in the list
            for (int i = 0; i < images.Count; i++)
            {
                images[i].Sequencia = i + 1;
            }

            try
            {
                await _minishop2023Context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle any exception that might occur during the save operation
                // Log the exception if necessary
                return false;
            }
        }

        //testar
        public async Task<List<ProductImage>> GetImagesByProductId(int productId)
        {
            //Recupera todas as imagens associadas aquele productId do banco de dados
            return await _minishop2023Context.ProductImages
                .Where(img => img.ProductId == productId)
                .ToListAsync();
        }

        //testar
        public async Task<bool> AtualizarSequenciaDeImagens(List<ProductImage> images)
        {
            if (images == null || images.Count == 0)
            {
                return false;
            }

            try
            {
                foreach (var image in images)
                {
                    _minishop2023Context.Entry(image).State = EntityState.Modified;
                }
                await _minishop2023Context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //testar
        public async Task EditarImagem(ProductImage productImage)
        {
            _minishop2023Context.Entry(productImage).State = EntityState.Modified;
            await _minishop2023Context.SaveChangesAsync();
        }

        //testar
        public async Task<int> GetHighestSequence(int productId)
        {
            var highestSequence = await _minishop2023Context.ProductImages
        .Where(pi => pi.ProductId == productId)
        .OrderByDescending(pi => pi.Sequencia)
        .Select(pi => pi.Sequencia)
        .FirstOrDefaultAsync();

            // Return 0 if no images are found for the given productId
            return highestSequence;
        }

    }

}
