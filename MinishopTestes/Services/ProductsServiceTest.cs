//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Minishop.DAL.Repositories;
//using Minishop.DAL.Base;
//using Minishop.Domain.DTO;
//using Minishop.Domain.Entity;
//using Minishop.Services;
//using Minishop.Services.Base;
//using Moq;
//using Xunit;
//using Minishop.DAL;

//namespace Minishop.Tests.Services
//{
//    public class ProductsServiceTests
//    {
//        private readonly Mock<IProductsRepository> _mockRepository;
//        private readonly Mock<Minishop2023Context> _mockContext;
//        private readonly ProductsService _service;

//        public ProductsServiceTests()
//        {
//            _mockRepository = new Mock<IProductsRepository>();
//            _mockContext = new Mock<Minishop2023Context>();
//            _service = new ProductsService(_mockContext.Object, _mockRepository.Object);
//        }

//        [Fact]
//        public async Task Contar_DeveRetornarProductCountResponseCorreto()
//        {
//            // Arrange
//            var expectedResponse = new ProductCountResponse
//            {
//                ItemCount = 10,
//                ActiveCount = 8,
//                InactiveCount = 2
//            };
//            _mockRepository.Setup(repo => repo.Contagem())
//                .ReturnsAsync(expectedResponse.ItemCount);
//            _mockRepository.Setup(repo => repo.ContagemProdutosAtivos())
//                .ReturnsAsync(expectedResponse.ActiveCount);
//            _mockRepository.Setup(repo => repo.ContagemProdutosInativos())
//                .ReturnsAsync(expectedResponse.InactiveCount);

//            // Act
//            ProductCountResponse response = await _service.Contar();

//            // Assert
//            Assert.Equal(expectedResponse, response);
//        }

//        [Fact]
//        public async Task Pesquisar_DeveRetornarServicePagedResponseCorreto()
//        {
//            // Arrange
//            var queryRequest = new PageQueryRequest
//            {
//                PaginaAtual = 1,
//                Quantidade = 10
//            };
//            var expectedResult = new List<Product>
//            {
//                new Product { Id = 1 },
//                new Product { Id = 2 }
//            };
//            var expectedResponse = new ServicePagedResponse<ProductsResponse>(
//                expectedResult.Select(p => new ProductsResponse(p)).ToList(),
//                expectedResult.Count,
//                queryRequest.PaginaAtual,
//                queryRequest.Quantidade
//            );
//            _mockRepository.Setup(repo => repo.Pesquisar(queryRequest.PaginaAtual, queryRequest.Quantidade))
//                .ReturnsAsync(expectedResult);
//            _mockRepository.Setup(repo => repo.Contagem())
//                .ReturnsAsync(expectedResult.Count);

//            // Act
//            ServicePagedResponse<ProductsResponse> response = await _service.Pesquisar(queryRequest);

//            // Assert
//            Assert.Equal(expectedResponse, response);
//        }

//        [Fact]
//        public async Task PesquisaPorId_DeveRetornarServiceResponseCorreto()
//        {
//            // Arrange
//            int productId = 1;
//            var expectedProduct = new Product { Id = productId };
//            var expectedResponse = new ServiceResponse<ProductsCompletoResponse>(
//                new ProductsCompletoResponse(expectedProduct)
//            );
//            _mockRepository.Setup(repo => repo.PesquisaPorId(productId))
//                .ReturnsAsync(expectedProduct);

//            // Act
//            ServiceResponse<ProductsCompletoResponse> response = await _service.PesquisaPorId(productId);

//            // Assert
//            Assert.Equal(expectedResponse, response);
//        }

//        [Fact]
//        public async Task PesquisaPorId_DeveRetornarServiceResponseVazioSeProdutoNaoEncontrado()
//        {
//            // Arrange
//            int productId = 1;
//            var expectedResponse = new ServiceResponse<ProductsCompletoResponse>(
//                mensagemDeErro: "Produto não encontrado"
//            );
//            _mockRepository.Setup(repo => repo.PesquisaPorId(productId))
//                .ReturnsAsync((Product)null);

//            // Act
//            ServiceResponse<ProductsCompletoResponse> response = await _service.PesquisaPorId(productId);

//            // Assert
//            Assert.Equal(expectedResponse, response);
//        }

//        [Fact]
//        public async Task Cadastrar_DeveRetornarServiceResponseCorreto()
//        {
//            // Arrange
//            var request = new ProductCreateRequest { /* mock request */ };
//            var product = new Product { /* mock product */ };
//            var expectedResponse = new ServiceResponse<ProductsResponse>(
//                new ProductsResponse(product)
//            );
//            _mockRepository.Setup(repo => repo.Cadastrar(product))
//                .ReturnsAsync(product);

//            // Act
//            ServiceResponse<ProductsResponse> response = await _service.Cadastrar(request);

//            // Assert
//            Assert.Equal(expectedResponse, response);
//        }

//        [Fact]
//        public async Task Editar_DeveRetornarServiceResponseCorreto()
//        {
//            // Arrange
//            int productId = 1;
//            var request = new ProductUpdateRequest { /* mock request */ };
//            var product = new Product { /* mock product */ };
//            var expectedResponse = new ServiceResponse<Product>(
//                product
//            );
//            _mockRepository.Setup(repo => repo.PesquisaPorId(productId))
//                .ReturnsAsync(product);
//            _mockRepository.Setup(repo => repo.Editar(productId, request))
//                .ReturnsAsync(product);

//            // Act
//            ServiceResponse<Product> response = await _service.Editar(productId, request);

//            // Assert
//            Assert.Equal(expectedResponse, response);
//        }

//        [Fact]
//        public async Task Editar_DeveRetornarServiceResponseVazioSeProdutoNaoEncontrado()
//        {
//            // Arrange
//            int productId = 1;
//            var request = new ProductUpdateRequest { /* mock request */ };
//            var expectedResponse = new ServiceResponse<Product>(
//                mensagemDeErro: "Produto não encontrado"
//            );
//            _mockRepository.Setup(repo => repo.PesquisaPorId(productId))
//                .ReturnsAsync((Product)null);

//            // Act
//            ServiceResponse<Product> response = await _service.Editar(productId, request);

//            // Assert
//            Assert.Equal(expectedResponse, response);
//        }
//    }
//}


