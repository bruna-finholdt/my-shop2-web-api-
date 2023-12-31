﻿using Minishop.DAL.Repositories;
using Minishop.DAL;
using Minishop.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Minishop.Services.Base;

namespace Minishop.Tests.Services
{
    public class ProductsServiceTest
    {
        private readonly Mock<IProductsRepository> _mockRepository;
        private readonly ProductsService _productsService;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly Mock<ISuppliersRepository> _mockSuppliersRepository;

        public ProductsServiceTest()
        {
            _mockRepository = new Mock<IProductsRepository>();
            _mockSuppliersRepository = new Mock<ISuppliersRepository>();
            _mockStorageService = new Mock<IStorageService>();
            _productsService = new ProductsService(_mockRepository.Object, _mockSuppliersRepository.Object, _mockStorageService.Object);
        }

        [Fact]
        public async Task Contar_DeveRetornarQuantidade()
        {
            // Arrange
            int expectedTotalCount = 20;
            int expectedActiveCount = 10;
            int expectedInactiveCount = 5;

            _mockRepository.Setup(repo => repo.Contagem()).ReturnsAsync(expectedTotalCount);
            _mockRepository.Setup(repo => repo.ContagemProdutosAtivos()).ReturnsAsync(expectedActiveCount);
            _mockRepository.Setup(repo => repo.ContagemProdutosInativos()).ReturnsAsync(expectedInactiveCount);

            // Act
            ProductCountResponse response = await _productsService.Contar();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedTotalCount, response.ItemCount);
            Assert.Equal(expectedActiveCount, response.ActiveCount);
            Assert.Equal(expectedInactiveCount, response.InactiveCount);
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarListagem()
        {
            // Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 2
            };

            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Prod1", SupplierId = 1, UnitPrice = 100, IsDiscontinued = false, PackageName = "1 pacote" },
                new Product { Id = 2, ProductName = "Prod2", SupplierId = 2, UnitPrice = 50, IsDiscontinued = false, PackageName = "1 pacote" },

            };

            _mockRepository.Setup(repo => repo.Pesquisar(1, 2)).ReturnsAsync(products);

            // Act
            var result = await _productsService.Pesquisar(queryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Conteudo.Count()); // Verifica se a quantidade de produtos retornados é a esperada.

            // Verifica se os dados dos produtos estão corretos
            var productsResponseList = result.Conteudo.ToList();
            Assert.Equal(products[0].Id, productsResponseList[0].Id);
            Assert.Equal(products[0].ProductName, productsResponseList[0].ProductName);
            Assert.Equal(products[0].SupplierId, productsResponseList[0].SupplierId);
            Assert.Equal(products[0].UnitPrice, productsResponseList[0].UnitPrice);
            Assert.Equal(products[0].IsDiscontinued, productsResponseList[0].IsDiscontinued);
            Assert.Equal(products[0].PackageName, productsResponseList[0].PackageName);

            Assert.Equal(products[1].Id, productsResponseList[1].Id);
            Assert.Equal(products[1].ProductName, productsResponseList[1].ProductName);
            Assert.Equal(products[1].SupplierId, productsResponseList[1].SupplierId);
            Assert.Equal(products[1].UnitPrice, productsResponseList[1].UnitPrice);
            Assert.Equal(products[1].IsDiscontinued, productsResponseList[1].IsDiscontinued);
            Assert.Equal(products[1].PackageName, productsResponseList[1].PackageName);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarProdutoQuandoIdExistente()
        {
            // Arrange
            int id = 1;
            var existingProduct = new Product
            {
                Id = id,
                ProductName = "Produto A",
                SupplierId = 1,
                UnitPrice = 10.0m,
                IsDiscontinued = false,
                PackageName = "Pacote 1"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingProduct);

            // Act
            var result = await _productsService.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingProduct.Id, result.Conteudo.Id);
            Assert.Equal(existingProduct.ProductName, result.Conteudo.ProductName);
            Assert.Equal(existingProduct.SupplierId, result.Conteudo.SupplierId);
            Assert.Equal(existingProduct.UnitPrice, result.Conteudo.UnitPrice);
            Assert.Equal(existingProduct.IsDiscontinued, result.Conteudo.IsDiscontinued);
            Assert.Equal(existingProduct.PackageName, result.Conteudo.PackageName);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarNuloQuandoIdNaoExistente()
        {
            // Arrange
            int idNaoExistente = 999; // ID que não existe no repositório
            _mockRepository.Setup(repo => repo.PesquisaPorId(idNaoExistente)).ReturnsAsync((Product)null);

            // Act
            var result = await _productsService.PesquisaPorId(idNaoExistente);

            // Assert
            Assert.Null(result.Conteudo);
            Assert.False(result.Sucesso);
            Assert.Equal("Produto não encontrado", result.Mensagem);
        }

        //CADASTRO DE PRODUTO
        [Fact]
        public async Task Cadastrar_DeveSalvarProdutoQuandoRequestValido()
        {
            // Arrange
            var request = new ProductCreateRequest
            {
                ProductName = "Product A",
                SupplierId = 1,
                UnitPrice = 50,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            _mockSuppliersRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync(new Supplier());


            Product savedProduct = null;
            _mockRepository.Setup(repo => repo.Cadastrar(It.IsAny<Product>()))
                .Callback<Product>(product => savedProduct = product)
                .ReturnsAsync((Product product) => product);

            // Act
            var response = await _productsService.Cadastrar(request);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(request.ProductName, savedProduct.ProductName);
            Assert.Equal(request.SupplierId, savedProduct.SupplierId);
            Assert.Equal(request.UnitPrice, savedProduct.UnitPrice);
            Assert.Equal(request.IsDiscontinued, savedProduct.IsDiscontinued);
            Assert.Equal(request.PackageName, savedProduct.PackageName);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoFornecedorNaoEncontrado()
        {
            // Arrange
            var request = new ProductCreateRequest
            {
                ProductName = "Product A",
                SupplierId = 99, // Um ID de fornecedor que não existe no banco de dados
                UnitPrice = 50,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            // Set up the behavior of ISuppliersRepository for the scenario where the supplier does not exist (returning null)
            _mockSuppliersRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync((Supplier)null);

            // Act
            var response = await _productsService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("Fornecedor não encontrado.", response.Mensagem);
        }

        //EDIÇÃO DE PRODUTO
        [Fact]
        public async Task Editar_DeveSalvarProdutoQuandoRequestValido()
        {
            // Arrange
            int id = 1;
            var request = new ProductUpdateRequest
            {
                ProductName = "Updated Product A",
                SupplierId = 2, // Novo ID de fornecedor existente no banco de dados
                UnitPrice = 60,
                IsDiscontinued = true,
                PackageName = "Updated Package 1"
            };

            var existingProduct = new Product
            {
                Id = id,
                ProductName = "Product A",
                SupplierId = 1,
                UnitPrice = 50,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingProduct);
            _mockSuppliersRepository.Setup(repo => repo.PesquisaPorId(request.SupplierId.Value)).ReturnsAsync(new Supplier()); // Use PesquisaPorId to check supplier existence

            Product savedProduct = null;
            _mockRepository.Setup(repo => repo.Editar(It.IsAny<Product>()))
                .Callback<Product>(product => savedProduct = product)
                .ReturnsAsync((Product product) => product);

            // Act
            var response = await _productsService.Editar(id, request);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(request.ProductName, savedProduct.ProductName);
            Assert.Equal(request.SupplierId, savedProduct.SupplierId);
            Assert.Equal(request.UnitPrice, savedProduct.UnitPrice);
            Assert.Equal(request.IsDiscontinued, savedProduct.IsDiscontinued);
            Assert.Equal(request.PackageName, savedProduct.PackageName);
        }

        [Fact]
        public async Task Editar_DeveRetornarErroQuandoProdutoNaoEncontrado()
        {
            // Arrange
            int id = 99; // ID de produto que não existe no banco de dados
            var request = new ProductUpdateRequest
            {
                ProductName = "Updated Product A",
                SupplierId = 2,
                UnitPrice = 60,
                IsDiscontinued = true,
                PackageName = "Updated Package 1"
            };

            Product existingProduct = null;
            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingProduct);

            // Act
            var response = await _productsService.Editar(id, request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("Produto não encontrado.", response.Mensagem);
            _mockRepository.Verify(repo => repo.Editar(It.IsAny<Product>()), Times.Never());
        }

        //CADASTRO DE IMAGEM DE PRODUTO
        [Fact]
        public async Task CadastrarImagem_DeveCadastrarImagensComSucesso()
        {
            //Arrange
            var productId = 1;
            var fileMock1 = new FormFile(Stream.Null, 0, 0, "fileMock1", "fileMock1.jpg");
            var fileMock2 = new FormFile(Stream.Null, 0, 0, "fileMock2", "fileMock2.jpg");

            _mockRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync(new Product { Id = productId });

            _mockStorageService.Setup(repo => repo.UploadFile(fileMock1, productId)).ReturnsAsync("urlFileMock1.jpg");
            _mockStorageService.Setup(repo => repo.UploadFile(fileMock2, productId)).ReturnsAsync("urlFileMock2.jpg");

            //Já existe o produto com uma imagem com valor de sequência 1 no banco. Cadastraremos a de valor 2 e 3 agora.
            _mockRepository.SetupSequence(repo => repo.GetHighestSequence(productId)).ReturnsAsync(1).ReturnsAsync(2);

            _mockRepository.Setup(repo => repo.CadastrarImagem(It.IsAny<ProductImage>()))
                .ReturnsAsync((ProductImage productImage) => productImage);

            //Act
            var response1 = await _productsService.CadastrarImagem(fileMock1, productId);
            var response2 = await _productsService.CadastrarImagem(fileMock2, productId);

            //Assert
            Assert.True(response1.Sucesso);
            Assert.True(response2.Sucesso);
            Assert.NotNull(response1.Conteudo);
            Assert.NotNull(response2.Conteudo);
            Assert.Equal("urlFileMock1.jpg", response1.Conteudo.Url);
            Assert.Equal("urlFileMock2.jpg", response2.Conteudo.Url);
            Assert.Equal(2, response1.Conteudo.Sequencia);//A próxima sequência após o 1 é o 2
            Assert.Equal(3, response2.Conteudo.Sequencia);//A próxima sequência após o 2 é o 3
        }

        [Fact]
        public async Task CadastrarImagem_DeveRetornarErroQuandoProductIdNaoExiste()
        {
            //Arrange
            var productId = 99;
            var fileMock = new FormFile(Stream.Null, 0, 0, "fileMock", "fileMock.jpg");

            //Simula o repository retornando null, indicando que o produto não existe
            _mockRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync((Product)null);

            //Act
            var response = await _productsService.CadastrarImagem(fileMock, productId);

            //Assert
            Assert.False(response.Sucesso);//Retorna false indicando erro
            Assert.Null(response.Conteudo);//Conteúdo null devido ao erro
            Assert.Equal("Produto não encontrado.", response.Mensagem);
        }

        [Fact]
        public async Task DeletarImagem_DeveRemoverImagensComSucesso()
        {
            // Arrange
            var productId = 1;
            var imageIdsToRemove = new List<int> { 1, 2, 3 };//IDs das imagens a serem removidas

            var productImages = new List<ProductImage>
        {
            new ProductImage { Id = 1, ProductId = productId, Url = "urlImage1" },
            new ProductImage { Id = 2, ProductId = productId, Url = "urlImage2" },
            new ProductImage { Id = 3, ProductId = productId, Url = "urlImage3" }
        };

            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });
            _mockRepository.Setup(repo => repo.GetImageById(It.IsAny<int>()))
                           .ReturnsAsync((int imageId) => productImages.FirstOrDefault(img => img.Id == imageId));
            _mockRepository.Setup(repo => repo.RemoverImagem(It.IsAny<int>()))
                           .ReturnsAsync(true);

            _mockStorageService.Setup(service => service.RemoveImageFromBucket(It.IsAny<string>()))
                               .ReturnsAsync(true);

            var request = new ProductImageDeleteRequest
            {
                ImageIdsToRemove = imageIdsToRemove
            };

            //Act
            var response = await _productsService.RemoverImagem(productId, request);

            //Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(0, response.Conteudo.Count);
            _mockRepository.Verify(repo => repo.RemoverImagem(1));
            _mockRepository.Verify(repo => repo.RemoverImagem(2));
            _mockRepository.Verify(repo => repo.RemoverImagem(3));
            _mockRepository.Verify(repo => repo.RemoverImagem(It.IsAny<int>()), Times.Exactly(3));
        }

        [Fact]
        public async Task DeletarImagem_DeveRetornarErroQuandoIdDeImagemNaoExiste()
        {
            //Arrange
            var productId = 1;

            //Mock imagens já existentes do produto com id 1
            var existingImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 2, Url = "url2", Sequencia = 2, ProductId = productId },
                new ProductImage { Id = 3, Url = "url3", Sequencia = 3, ProductId = productId }
            };

            //Mock do ProductImageDeleteRequest
            var model = new ProductImageDeleteRequest
            {
                ImageIdsToRemove = new List<int> { 4 } //ID 4 não existe nas imagens existentes
            };

            //Simula o repository
            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });

            _mockRepository.Setup(repo => repo.RemoverImagem(It.IsAny<int>())).ReturnsAsync(false);//Simula falha ao remover imagem

            //GetImagesByProductId (antes de remover imagens)
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId))
                .ReturnsAsync(existingImages);//retorna a lista de imagens original

            _mockRepository.Setup(repo => repo.ReorganizarSequenciaDeImagens(productId)).ReturnsAsync(true);

            //Act
            var response = await _productsService.RemoverImagem(productId, model);

            //Assert
            Assert.False(response.Sucesso);
            Assert.Equal("Imagem com ID 4 não existente.", response.Mensagem);
        }

        [Fact]
        public async Task DeletarImagem_DeveRetornarErroQuandoIdDeImagemNaoPertenceAoProduto()
        {
            //Arrange
            var productId = 1;

            //Mock imagens já existentes do produto com id 1
            var existingImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 2, Url = "url2", Sequencia = 2, ProductId = productId },
                new ProductImage { Id = 3, Url = "url3", Sequencia = 3, ProductId = productId },
                new ProductImage { Id = 4, Url = "url4", Sequencia = 1, ProductId = 2 },
            };

            //Mock do ProductImageDeleteRequest
            var model = new ProductImageDeleteRequest
            {
                ImageIdsToRemove = new List<int> { 2, 4 } //ID 4 não pertence ao produto de id 1
            };

            //Simula o repository
            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });

            //Simula a busca por imagem existente
            _mockRepository.Setup(repo => repo.GetImageById(It.IsAny<int>()))
                           .ReturnsAsync((int imageId) => existingImages.FirstOrDefault(img => img.Id == imageId));

            _mockRepository.Setup(repo => repo.RemoverImagem(It.IsAny<int>())).ReturnsAsync(true);

            //Simula reorganização das sequências
            _mockRepository.Setup(repo => repo.ReorganizarSequenciaDeImagens(productId)).ReturnsAsync(true);

            //Act
            var response = await _productsService.RemoverImagem(productId, model);

            //Assert
            Assert.False(response.Sucesso);
            Assert.Equal("A imagem com ID 4 não pertence ao produto com ID 1.", response.Mensagem);
        }

        [Fact]
        public async Task AlterarOrdemImagens_DeveAtualizarSequenciaDeImagensComSucesso()
        {
            //Arrange
            var productId = 1;
            var model = new ProductImageOrderUpdateRequest
            {
                ImageIds = new List<int> { 2, 1 }//A ordem das imagens será invertida
            };
            var images = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 2, Url = "url2", Sequencia = 2, ProductId = productId }
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId)).ReturnsAsync(images);

            //Atualização da sequência das imagens 
            _mockRepository.Setup(repo => repo.ReorganizarSequenciaDeImagens(productId)).ReturnsAsync(true);

            //Act
            var response = await _productsService.AlterarOrdemImagens(productId, model);

            //Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(2, response.Conteudo.Count);
            Assert.Equal(2, response.Conteudo[0].Sequencia);
            Assert.Equal(1, response.Conteudo[1].Sequencia);
        }

        [Fact]
        public async Task AlterarOrdemImagens_DeveRetornarErroQuandoListaDeIdsContemIdsInvalidos()
        {
            //Arrange
            var productId = 1;
            var model = new ProductImageOrderUpdateRequest
            {
                ImageIds = new List<int> { 2, 1 }
            };
            var images = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 3, Url = "url3", Sequencia = 2, ProductId = productId }//Id 3 e não 2
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId)).ReturnsAsync(images);

            //Act
            var response = await _productsService.AlterarOrdemImagens(productId, model);

            //Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("A lista de IDs de imagens contém IDs inválidos ou não pertencentes ao produto.", response.Mensagem);
        }
    }
}
