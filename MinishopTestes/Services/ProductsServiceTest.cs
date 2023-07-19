using Minishop.DAL.Repositories;
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

namespace Minishop.Tests.Services
{
    public class ProductsServiceTest
    {
        private readonly Mock<IProductsRepository> _mockRepository;
        private readonly ProductsService _productsService;

        public ProductsServiceTest()
        {
            _mockRepository = new Mock<IProductsRepository>();
            _productsService = new ProductsService(_mockRepository.Object);
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

            _mockRepository.Setup(repo => repo.VerificarFornecedorExistente(request.SupplierId)).ReturnsAsync(true);

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

            _mockRepository.Setup(repo => repo.VerificarFornecedorExistente(request.SupplierId)).ReturnsAsync(false);

            // Act
            var response = await _productsService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("Fornecedor não encontrado.", response.Mensagem);
        }

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
            _mockRepository.Setup(repo => repo.VerificarFornecedorExistente(request.SupplierId)).ReturnsAsync(true);

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
        }


    }
}
