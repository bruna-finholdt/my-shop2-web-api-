﻿using k8s.Models;
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

        public async Task<ServiceResponse<ProductImageResponse>> CadastrarImagem(IFormFile file, int productId)
        {
            //Valida se o produto com o ID fornecido existe na base de dados
            var existingProduct = await _productsRepository.PesquisaPorId(productId);
            if (existingProduct == null)
            {
                return new ServiceResponse<ProductImageResponse>("Produto não encontrado.");
            }

            //Obtém do db o valor de Sequencia mais alto para aquele produto
            int highestSequence = await _productsRepository.GetHighestSequence(productId);

            //Incrementa em 1 o valor da highestSequence
            int nextSequence = highestSequence + 1;

            //Cadastra a imagem do produto no bucket e obtém a URL da imagem (key)
            string imageUrl = await _storageService.UploadFile(file, productId);

            //Cria uma nova instância de ProductImage para salvar no db
            var productImage = new ProductImage
            {
                Url = imageUrl,
                ProductId = productId,
                Sequencia = nextSequence
            };

            //Chama o método do repository, que salva a imagem do produto no db
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

            //Atualiza os campos do produto com os valores do modelo, se eles não forem modificados
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

            //Converte o updated product em DTO response
            var productResponse = new ProductsCompletoResponse(updatedProduct);

            return new ServiceResponse<ProductsCompletoResponse>(productResponse);
        }

        public async Task<ServiceResponse<List<ProductImageResponse>>> RemoverImagem(int productId, ProductImageDeleteRequest model)
        {
            var existingProduct = await _productsRepository.PesquisaPorId(productId);
            if (existingProduct == null)
            {
                return new ServiceResponse<List<ProductImageResponse>>("Produto não encontrado.");
            }

            // Remove imagem(ens) existente(s)
            if (model.ImageIdsToRemove == null || !model.ImageIdsToRemove.Any())
            {
                return new ServiceResponse<List<ProductImageResponse>>("Nenhum ID de imagem fornecido para remoção.");
            }

            foreach (var imageId in model.ImageIdsToRemove)
            {
                // Verifica se a imagem existe para algum produto
                var imageToRemove = await _productsRepository.GetImageById(imageId);
                if (imageToRemove == null)
                {
                    return new ServiceResponse<List<ProductImageResponse>>($"Imagem com ID {imageId} não existente.");
                }

                // Verifica se a imagem pertence ao produto
                if (imageToRemove.ProductId != productId)
                {
                    return new ServiceResponse<List<ProductImageResponse>>($"A imagem com ID {imageId} não pertence ao produto com ID {productId}.");
                }

                // Remove a imagem do db
                bool imageRemoved = await _productsRepository.RemoverImagem(imageId);

                if (!imageRemoved)
                {
                    return new ServiceResponse<List<ProductImageResponse>>($"Erro ao remover imagem com ID {imageId}.");
                }

                // Reorganiza as sequências das imagens restantes usando o método existente no repository
                await _productsRepository.ReorganizarSequenciaDeImagens(productId);
            }

            var remainingImages = await _productsRepository.GetImagesByProductId(productId);

            // Verifica se existem imagens restantes
            if (remainingImages == null || !remainingImages.Any())
            {
                // Nenhuma imagem restante, retorna uma lista vazia
                return new ServiceResponse<List<ProductImageResponse>>(new List<ProductImageResponse>());
            }

            // Return success response with the list of remaining images
            var response = new ServiceResponse<List<ProductImageResponse>>(remainingImages.Select(image => new ProductImageResponse(image)).ToList());
            return response;
        }

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

            //Verifica se os IDs fornecidos são válidos e pertencem ao produto
            var validImageIds = images.Select(img => img.Id);
            if (!model.ImageIds.All(id => validImageIds.Contains(id)))
            {
                return new ServiceResponse<List<ProductImageResponse>>("A lista de IDs de imagens contém IDs inválidos ou não pertencentes ao produto.");
            }

            //Atualiza a sequência de cada imagem de acordo com a lista de IDs fornecida
            for (int i = 0; i < model.ImageIds.Count; i++)
            {
                var imageId = model.ImageIds[i];
                var image = images.FirstOrDefault(img => img.Id == imageId);
                if (image != null && image.Sequencia != (i + 1))
                {
                    image.Sequencia = i + 1; //Adiciona 1 ao valor do índice para manter a sequência iniciando em 1
                    await _productsRepository.EditarImagem(image);
                }
            }

            //Obtém todas as imagens atualizadas associadas ao produto após a alteração de ordem
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
