using k8s.Models;
using Microsoft.EntityFrameworkCore;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.DTO.Validation;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ISuppliersRepository _supplierRepository;
        private readonly IStorageService _storageService;

        public ProductsService(IProductsRepository productsRepository, ISuppliersRepository supplierRepository, IStorageService storageService)
        {
            _productsRepository = productsRepository;
            _supplierRepository = supplierRepository;
            _storageService = storageService;
        }

        public async Task<ProductCountResponse> Contar()
        {
            int quantidadeTotal = await _productsRepository.Contagem();
            int quantidadeProdutosAtivos = await _productsRepository.ContagemProdutosAtivos();
            int quantidadeProdutosInativos = await _productsRepository.ContagemProdutosInativos();

            var response = new ProductCountResponse
            {
                ItemCount = quantidadeTotal,
                ActiveCount = quantidadeProdutosAtivos,
                InactiveCount = quantidadeProdutosInativos
            };

            return response;
        }

        public async Task<ServicePagedResponse<ProductsResponse>> Pesquisar(PageQueryRequest queryResquest)
        {
            //Lista Products com paginação
            {
                // Consulta itens no banco
                var listaPesquisa = await _productsRepository.Pesquisar(
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
                // Conta itens do banco
                var contagem = await _productsRepository.Contagem();
                // Transforma Product em ProductResponse
                var listaConvertida = listaPesquisa
                    .Select(product => new ProductsResponse(product));

                // Cria resultado com paginação
                return new ServicePagedResponse<ProductsResponse>(
                    listaConvertida,
                    contagem,
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
            }
            //No método de listagem de todos os products, os usos do método Select da biblioteca Linq
            //funcionam como um transformador para cada objeto da lista;

        }

        public async Task<ServiceResponse<ProductsCompletoResponse>> PesquisaPorId(int id)
        //Para usar um método async, devemos colocar async na assinatura, Task<> no retorno e colocar o
        //await na chamada de qualquer método async interno.
        {
            var product = await _productsRepository.PesquisaPorId(id);


            if (product == null)
            {
                return new ServiceResponse<ProductsCompletoResponse>(
                    "Produto não encontrado"
                );
            }


            return new ServiceResponse<ProductsCompletoResponse>(
                new ProductsCompletoResponse(product)
            );

        }

        public async Task<ServiceResponse<ProductsCompletoResponse>> Cadastrar(ProductCreateRequest novo)
        {

            if (novo.UnitPrice == null || !novo.UnitPrice.HasValue)
            {
                return new ServiceResponse<ProductsCompletoResponse>("O preço deve ser informado.");
            }

            if (novo.UnitPrice.Value <= 0)
            {
                return new ServiceResponse<ProductsCompletoResponse>("O preço deve ter um valor positivo.");
            }

            //Verifica se o Id do supplier enviado existe na base de dados
            var supplier = await _supplierRepository.PesquisaPorId(novo.SupplierId);
            if (supplier == null)
            {
                return new ServiceResponse<ProductsCompletoResponse>("Fornecedor não encontrado.");
            }

            var produto = new Product
            {
                ProductName = novo.ProductName,
                SupplierId = novo.SupplierId,
                UnitPrice = novo.UnitPrice,
                IsDiscontinued = novo.IsDiscontinued,
                PackageName = novo.PackageName,
                Supplier = supplier,
            };

            var createdProduct = await _productsRepository.Cadastrar(produto);

            if (createdProduct == null)
            {
                return new ServiceResponse<ProductsCompletoResponse>("Erro ao cadastrar produto.");
            }

            var response = new ProductsCompletoResponse(createdProduct);

            return new ServiceResponse<ProductsCompletoResponse>(response);
        }

        //testar
        public async Task<ServiceResponse<ProductImageResponse>> CadastrarImagem(IFormFile file, int productId)
        {

            //Valida se o produto com o ID fornecido existe na base de dados
            var existingProduct = await _productsRepository.PesquisaPorId(productId);
            if (existingProduct == null)
            {
                return new ServiceResponse<ProductImageResponse>("Produto não encontrado.");
            }

            // Get the highest sequence value for the given productId from the database
            int highestSequence = await _productsRepository.GetHighestSequence(productId);

            // Increment the sequence to determine the next value
            int nextSequence = highestSequence + 1;

            //Cadastra a imagem do produto no bucket e obtém a URL da imagem (key)
            string imageUrl = await _storageService.UploadFile(file, productId);

            //Cria uma nova instância de ProductImage para salvar no banco de dados
            var productImage = new ProductImage
            {
                Url = imageUrl,
                ProductId = productId,
                Sequencia = nextSequence
            };

            //Chama o método do repository, que salva a imagem do produto no banco de dados
            var createdProductImage = await _productsRepository.CadastrarImagem(productImage);

            if (createdProductImage == null)
            {
                return new ServiceResponse<ProductImageResponse>("Erro ao cadastrar imagem do produto.");
            }

            var response = new ProductImageResponse(createdProductImage);

            return new ServiceResponse<ProductImageResponse>(response);
        }

        public async Task<ServiceResponse<ProductsCompletoResponse>> Editar(int id, ProductUpdateRequest model)
        {

            if (model.UnitPrice == null || !model.UnitPrice.HasValue)
            {
                return new ServiceResponse<ProductsCompletoResponse>("O preço deve ser informado.");
            }

            if (model.UnitPrice.Value <= 0)
            {
                return new ServiceResponse<ProductsCompletoResponse>("O preço deve ter um valor positivo.");
            }

            var existingProduct = await _productsRepository.PesquisaPorId(id);
            if (existingProduct == null)
            {
                return new ServiceResponse<ProductsCompletoResponse>("Produto não encontrado.");
            }

            //Verifica se o ID do fornecedor enviado existe na base de dados 
            if (model.SupplierId.HasValue)
            {
                var supplier = await _supplierRepository.PesquisaPorId(model.SupplierId.Value);
                if (supplier == null)
                {
                    return new ServiceResponse<ProductsCompletoResponse>("Fornecedor não encontrado.");
                }
                existingProduct.SupplierId = model.SupplierId.Value;
                existingProduct.Supplier = supplier;
            }

            //Atualiza os campos do produto com os valores do modelo, se eles não forem nulos ou vazios
            if (!string.IsNullOrWhiteSpace(model.ProductName))
            {
                existingProduct.ProductName = model.ProductName;
            }

            if (model.UnitPrice.HasValue)
            {
                existingProduct.UnitPrice = model.UnitPrice.Value;
            }

            if (model.IsDiscontinued.HasValue)
            {
                existingProduct.IsDiscontinued = model.IsDiscontinued.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.PackageName))
            {
                existingProduct.PackageName = model.PackageName;
            }

            //Chama o método Editar do repository para salvar as alterações no banco de dados
            var updatedProduct = await _productsRepository.Editar(existingProduct);

            if (updatedProduct == null)
            {
                return new ServiceResponse<ProductsCompletoResponse>("Erro ao editar o produto.");
            }

            ////Obtém as imagens atualizadas daquele produto específico
            //var updatedImages = await _productsRepository.GetImagesByProductId(updatedProduct.Id);

            ////Converte o updated product em DTO response
            var productResponse = new ProductsCompletoResponse(updatedProduct);
            //var imageResponses = updatedImages.Select(img => new ProductImageResponse(img)).ToList();

            ////Cria o ProductAndImageResponse para incluir ambos os responses, do produto e das imagens 
            //var response = new ProductAndImageResponse(productResponse, imageResponses);

            return new ServiceResponse<ProductsCompletoResponse>(productResponse);
        }

        //testar
        public async Task<ServiceResponse<List<ProductImageResponse>>> EditarImagem(int productId, ProductImageUpdateRequest model)
        {
            var existingProduct = await _productsRepository.PesquisaPorId(productId);
            if (existingProduct == null)
            {
                return new ServiceResponse<List<ProductImageResponse>>("Produto não encontrado.");
            }

            // Validate the format of new images (PNG, JPG, or JPEG)
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
            if (model.NewImages != null && model.NewImages.Any())
            {
                foreach (IFormFile file in model.NewImages)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return new ServiceResponse<List<ProductImageResponse>>("Formato de arquivo não suportado. Apenas arquivos PNG, JPG e JPEG são permitidos.");
                    }
                }
            }

            // Remove images
            if (model.ImageIdsToRemove != null && model.ImageIdsToRemove.Any())
            {
                foreach (var imageId in model.ImageIdsToRemove)
                {
                    // Remove the image from the bucket and the database
                    bool imageRemoved = await _productsRepository.RemoverImagem(imageId);

                    // Verifies if the image was removed successfully
                    if (!imageRemoved)
                    {
                        return new ServiceResponse<List<ProductImageResponse>>($"Erro ao remover imagem com ID {imageId}.");
                    }
                }

                // Reorganiza as sequências of images after removing
                bool reorganized = await _productsRepository.ReorganizarSequenciaDeImagens(productId);

                if (!reorganized)
                {
                    return new ServiceResponse<List<ProductImageResponse>>("Erro ao reorganizar as sequências das imagens após a remoção.");
                }
            }

            // Get the current images associated with the product
            var existingImages = await _productsRepository.GetImagesByProductId(productId);

            // Calculate the next sequence value based on the count of existing images
            int nextSequence = existingImages.Count + 1;

            // Add new images
            if (model.NewImages != null && model.NewImages.Any())
            {
                foreach (IFormFile file in model.NewImages)
                {
                    // Cadastra a nova imagem do produto no bucket e obtém a URL da imagem
                    string imageUrl = await _storageService.UploadFile(file, productId);

                    // Cria uma nova instância de ProductImage para salvar no banco de dados
                    var productImage = new ProductImage
                    {
                        Url = imageUrl,
                        ProductId = productId,
                        Sequencia = nextSequence
                    };

                    // Chama o método do repository para salvar a imagem do produto no banco de dados
                    await _productsRepository.CadastrarImagem(productImage);

                    nextSequence++; // Increment the nextSequence for the next iteration
                }
            }

            // Reorganiza as sequências das imagens after adding new images
            bool reorganizedAfterAdd = await _productsRepository.ReorganizarSequenciaDeImagens(productId);

            if (!reorganizedAfterAdd)
            {
                return new ServiceResponse<List<ProductImageResponse>>("Erro ao reorganizar as sequências das imagens após a adição.");
            }

            // Obtém todas as imagens atualizadas associadas ao produto após as edições
            var updatedImages = await _productsRepository.GetImagesByProductId(productId);

            var response = new List<ProductImageResponse>();
            foreach (var image in updatedImages)
            {
                response.Add(new ProductImageResponse(image));
            }

            return new ServiceResponse<List<ProductImageResponse>>(response);
        }

        //testar
        public async Task<ServiceResponse<List<ProductImageResponse>>> AlterarOrdemImagens(int productId, ProductImageOrderUpdateRequest model)
        {
            var existingProduct = await _productsRepository.PesquisaPorId(productId);
            if (existingProduct == null)
            {
                return new ServiceResponse<List<ProductImageResponse>>("Produto não encontrado.");
            }

            var images = await _productsRepository.GetImagesByProductId(productId);

            if (model.ImageIds == null || model.ImageIds.Count != images.Count)
            {
                return new ServiceResponse<List<ProductImageResponse>>("A lista de IDs de imagens fornecida não corresponde à quantidade de imagens do produto.");
            }

            // Verifica se os IDs fornecidos são válidos e pertencem ao produto
            var validImageIds = images.Select(img => img.Id);
            if (!model.ImageIds.All(id => validImageIds.Contains(id)))
            {
                return new ServiceResponse<List<ProductImageResponse>>("A lista de IDs de imagens contém IDs inválidos ou não pertencentes ao produto.");
            }

            // Atualiza a sequência de cada imagem de acordo com a lista de IDs fornecida
            for (int i = 0; i < model.ImageIds.Count; i++)
            {
                var imageId = model.ImageIds[i];
                var image = images.FirstOrDefault(img => img.Id == imageId);
                if (image != null && image.Sequencia != (i + 1))
                {
                    image.Sequencia = i + 1; // Adiciona 1 ao valor do índice para manter a sequência iniciando em 1
                    await _productsRepository.EditarImagem(image);
                }
            }

            // Obtém todas as imagens atualizadas associadas ao produto após a alteração de ordem
            var updatedImages = await _productsRepository.GetImagesByProductId(productId);

            var response = new List<ProductImageResponse>();
            foreach (var image in updatedImages)
            {
                response.Add(new ProductImageResponse(image));
            }

            return new ServiceResponse<List<ProductImageResponse>>(response);
        }

        public async Task<ServiceResponse<ProductsCompletoResponse>> PesquisaPorIdCompleto(int id)
        {
            var productResponse = await PesquisaPorId(id);
            if (!productResponse.Sucesso)
            {
                return new ServiceResponse<ProductsCompletoResponse>(productResponse.Mensagem);
            }

            var product = productResponse.Conteudo;

            var imagesResponse = await _productsRepository.GetImagesByProductId(id);

            if (!imagesResponse.Any())
            {
                return new ServiceResponse<ProductsCompletoResponse>("Não há imagens associadas a este produto.");
            }

            product.Images = imagesResponse.Select(img => new ProductImageResponse(img)).ToList();

            return new ServiceResponse<ProductsCompletoResponse>(product);
        }



    }
}
