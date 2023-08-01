using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minishop.Controllers;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;
using Moq;
using Xunit;

namespace Minishop.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductsService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductsService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [Fact]
        public async Task ObterContagem_DeveRetornarOk()
        {
            // Arrange
            var expectedTotalCount = 100;
            var expectedActiveCount = 80;
            var expectedInactiveCount = 20;

            var productCountResponse = new ProductCountResponse
            {
                ItemCount = expectedTotalCount,
                ActiveCount = expectedActiveCount,
                InactiveCount = expectedInactiveCount
            };

            _mockService.Setup(service => service.Contar())
                .ReturnsAsync(productCountResponse);

            // Act
            var result = await _controller.ObterContagem();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var response = (ProductCountResponse)okResult.Value; // Convert to ProductCountResponse

            // Check if the response values match the expected counts
            Assert.Equal(expectedTotalCount, response.ItemCount);
            Assert.Equal(expectedActiveCount, response.ActiveCount);
            Assert.Equal(expectedInactiveCount, response.InactiveCount);
        }

        [Fact]
        public async Task Get_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 10
            };

            var products = new List<ProductsResponse>
    {
        new ProductsResponse(new Product
        {
            Id = 1,
            ProductName = "Product 1",
            SupplierId = 1,
            UnitPrice = 10.99m,
            IsDiscontinued = false,
            PackageName = "Package 1"
        }),

        new ProductsResponse(new Product
        {
            Id = 2,
            ProductName = "Product 2",
            SupplierId = 2,
            UnitPrice = 5.99m,
            IsDiscontinued = true,
            PackageName = "Package 2"
        }),
    };

            var response = new ServicePagedResponse<ProductsResponse>(
                products,
                10,
                1,
                10
            );

            _mockService.Setup(service => service.Pesquisar(queryRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Get(queryRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            var resultValue = okResult.Value as ServicePagedResponse<ProductsResponse>;

            Assert.Equal(response.Conteudo, resultValue.Conteudo);
            Assert.Equal(response.PaginaAtual, resultValue.PaginaAtual);
            Assert.Equal(response.TotalPaginas, resultValue.TotalPaginas);
        }

        [Fact]
        public async Task GetById_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;

            var product = new Product
            {
                Id = id,
                ProductName = "Product 1",
                SupplierId = 1,
                UnitPrice = 10.99m,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            var response = new ServiceResponse<ProductsCompletoResponse>(new ProductsCompletoResponse(product));

            _mockService.Setup(service => service.PesquisaPorIdCompleto(id))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFoundQuandoIdForInvalido()
        {
            // Arrange
            int id = 5;

            string mensagem = "Produto não encontrado";

            var retorno = new ServiceResponse<ProductsCompletoResponse>(mensagem);

            _mockService.Setup(service => service.PesquisaPorIdCompleto(id))
                .ReturnsAsync(retorno);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<ProductsCompletoResponse>>(NotFoundResult.Value);

            Assert.False(response.Sucesso);
            Assert.Equal(mensagem, response.Mensagem);
        }

        [Fact]
        public async Task Post_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            var productCreateRequest = new ProductCreateRequest
            {
                ProductName = "New Product",
                SupplierId = 1,
                UnitPrice = 19.99m,
                IsDiscontinued = false,
                PackageName = "New Package"
            };

            var product = new Product
            {
                Id = 1,
                ProductName = "New Product",
                SupplierId = 1,
                UnitPrice = 19.99m,
                IsDiscontinued = false,
                PackageName = "New Package"
            };

            var expectedResponse = new ServiceResponse<ProductsCompletoResponse>(new ProductsCompletoResponse(product));

            _mockService.Setup(service => service.Cadastrar(productCreateRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Post(productCreateRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ServiceResponse<ProductsCompletoResponse>; // Extrair o ServiceResponse<ProductsCompletoResponse>
            Assert.Equal(expectedResponse.Conteudo, response.Conteudo); // Comparar apenas a propriedade Conteudo
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            var productCreateRequest = new ProductCreateRequest
            {
                // Deixando o ProductName vazio para simular um modelo inválido
                ProductName = "",
                SupplierId = 1,
                UnitPrice = 19.99m,
                IsDiscontinued = false,
                PackageName = "New Package"
            };

            _controller.ModelState.AddModelError("ProductName", "O nome do produto é obrigatório.");

            // Act
            var result = await _controller.Post(productCreateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);

            var error = badRequestResult.Value as SerializableError;
            Assert.True(error.ContainsKey("ProductName"));
        }

        [Fact]
        public async Task Put_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;

            var productUpdateRequest = new ProductUpdateRequest
            {
                ProductName = "Updated Product",
                SupplierId = 2,
                UnitPrice = 29.99m,
                IsDiscontinued = true,
                PackageName = "Updated Package"
            };

            var product = new Product
            {
                Id = id,
                ProductName = "Updated Product",
                SupplierId = 2,
                UnitPrice = 29.99m,
                IsDiscontinued = true,
                PackageName = "Updated Package"
            };

            var expectedResponse = new ServiceResponse<ProductsCompletoResponse>(new ProductsCompletoResponse(product));

            _mockService.Setup(service => service.Editar(id, productUpdateRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Put(id, productUpdateRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ServiceResponse<ProductsCompletoResponse>; // Extrair o ServiceResponse<ProductsCompletoResponse>
            Assert.Equal(expectedResponse.Conteudo, response.Conteudo); // Comparar apenas a propriedade Conteudo
        }


        [Fact]
        public async Task Put_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            int id = 1;

            var productUpdateRequest = new ProductUpdateRequest
            {
                // Deixando o ProductName vazio para simular um modelo inválido
                ProductName = "",
                SupplierId = 2,
                UnitPrice = 29.99m,
                IsDiscontinued = true,
                PackageName = "Updated Package"
            };

            _controller.ModelState.AddModelError("ProductName", "O nome do produto é obrigatório.");

            // Act
            var result = await _controller.Put(id, productUpdateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);

            var error = badRequestResult.Value as SerializableError;
            Assert.True(error.ContainsKey("ProductName"));
        }




    }
}



