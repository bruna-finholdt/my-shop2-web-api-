//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Minishop.Controllers;
//using Minishop.Domain.DTO;
//using Minishop.Domain.Entity;
//using Minishop.Services;
//using Minishop.Services.Base;
//using Moq;
//using Xunit;

//namespace Minishop.Tests.Controllers
//{
//    public class ProductsControllerTests
//    {
//        private readonly Mock<IProductsService> _mockService;
//        private readonly ProductsController _controller;

//        public ProductsControllerTests()
//        {
//            _mockService = new Mock<IProductsService>();
//            _controller = new ProductsController(_mockService.Object);
//        }

//        [Fact]
//        public async Task ObterContagem_DeveRetornarOk()
//        {
//            // Arrange
//            var response = new ProductCountResponse
//            {
//                ItemCount = 10,
//                ActiveCount = 8,
//                InactiveCount = 2
//            };

//            _mockService.Setup(service => service.Contar())
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.ObterContagem();

//            // Assert
//            Assert.IsType<OkObjectResult>(result.Result);
//            var okResult = result.Result as OkObjectResult;
//            Assert.Equal(response, okResult.Value);
//        }

//        [Fact]
//        public async Task Get_DeveRetornarOkQuandoServicoRetornarSucesso()
//        {
//            // Arrange
//            var queryRequest = new PageQueryRequest
//            {
//                PaginaAtual = 1,
//                Quantidade = 10
//            };
//            var response = new ServicePagedResponse<ProductsResponse>(
//                new ProductsResponse[] { /* mock response */ },
//                10,
//                1,
//                10
//            );

//            _mockService.Setup(service => service.Pesquisar(queryRequest))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.Get(queryRequest);

//            // Assert
//            Assert.IsType<OkObjectResult>(result);
//            var okResult = result as OkObjectResult;
//            Assert.Equal(response, okResult.Value);
//        }

//        [Fact]
//        public async Task Get_DeveRetornarBadRequestQuandoServicoRetornarFalha()
//        {
//            // Arrange
//            var queryRequest = new PageQueryRequest
//            {
//                PaginaAtual = 1,
//                Quantidade = 10
//            };
//            var response = new ServicePagedResponse<ProductsResponse>(
//                mensagemDeErro: "Erro na pesquisa"
//            );

//            _mockService.Setup(service => service.Pesquisar(queryRequest))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.Get(queryRequest);

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);
//            var badRequestResult = result as BadRequestObjectResult;
//            Assert.Equal(response, badRequestResult.Value);
//        }

//        [Fact]
//        public async Task GetById_DeveRetornarOkQuandoServicoRetornarSucesso()
//        {
//            // Arrange
//            int id = 1;
//            var product = new Product
//            {
//                Id = 1,
//                ProductName = "Test",
//                IsDiscontinued = true,
//                OrderItems = new[]
//                {
//                   new OrderItem
//                   {
//                       Id = 1,
//                   }
//                },
//                PackageName = "Test",
//                Supplier = new Supplier { Id = 1 },
//                SupplierId = 1,
//                UnitPrice = 10,

//            };
//            var response = new ServiceResponse<ProductsCompletoResponse>(
//                new ProductsCompletoResponse(product)
//            );

//            _mockService.Setup(service => service.PesquisaPorId(id))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.GetById(id.ToString());

//            // Assert
//            Assert.IsType<OkObjectResult>(result);
//            var okResult = result as OkObjectResult;
//            Assert.Equal(response.Conteudo, okResult.Value);
//        }

//        [Fact]
//        public async Task GetById_DeveRetornarNotFoundQuandoServicoRetornarFalha()
//        {
//            // Arrange
//            int id = 1;
//            var response = new ServiceResponse<ProductsCompletoResponse>(
//                mensagemDeErro: "Produto não encontrado"
//            );

//            _mockService.Setup(service => service.PesquisaPorId(id))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.GetById(id.ToString());

//            // Assert
//            Assert.IsType<NotFoundObjectResult>(result);
//            var notFoundResult = result as NotFoundObjectResult;
//            Assert.Equal(response, notFoundResult.Value);
//        }

//        [Fact]
//        public async Task Post_DeveRetornarOkQuandoServicoRetornarSucesso()
//        {
//            // Arrange
//            var request = new ProductCreateRequest { /* mock request */ };
//            var product = new Product { /* mock product */ };
//            var response = new ServiceResponse<ProductsResponse>(
//                new ProductsResponse(product)
//            );

//            _mockService.Setup(service => service.Cadastrar(request))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.Post(request);

//            // Assert
//            Assert.IsType<OkObjectResult>(result);
//            var okResult = result as OkObjectResult;
//            Assert.Equal(response.Conteudo, okResult.Value);
//        }

//        [Fact]
//        public async Task Post_DeveRetornarBadRequestQuandoModelInvalido()
//        {
//            // Arrange
//            var request = new ProductCreateRequest { /* mock invalid request */ };
//            _controller.ModelState.AddModelError("PropertyName", "Erro de validação");

//            // Act
//            var result = await _controller.Post(request);

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);
//            var badRequestResult = result as BadRequestObjectResult;
//            Assert.IsType<SerializableError>(badRequestResult.Value);
//        }

//        [Fact]
//        public async Task Put_DeveRetornarOkQuandoServicoRetornarSucesso()
//        {
//            // Arrange
//            int id = 1;
//            var request = new ProductUpdateRequest { /* mock request */ };
//            var product = new Product { /* mock product */ };
//            var response = new ServiceResponse<Product>(
//                new Product(/* mock response */)
//            );

//            _mockService.Setup(service => service.Editar(id, request))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.Put(id, request);

//            // Assert
//            Assert.IsType<OkObjectResult>(result);
//            var okResult = result as OkObjectResult;
//            Assert.Equal(response.Conteudo, okResult.Value);
//        }

//        [Fact]
//        public async Task Put_DeveRetornarBadRequestQuandoModelInvalido()
//        {
//            // Arrange
//            int id = 1;
//            var request = new ProductUpdateRequest { /* mock invalid request */ };
//            _controller.ModelState.AddModelError("PropertyName", "Erro de validação");

//            // Act
//            var result = await _controller.Put(id, request);

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);
//            var badRequestResult = result as BadRequestObjectResult;
//            Assert.IsType<SerializableError>(badRequestResult.Value);
//        }

//        [Fact]
//        public async Task Put_DeveRetornarBadRequestQuandoServicoRetornarFalha()
//        {
//            // Arrange
//            int id = 1;
//            var request = new ProductUpdateRequest { /* mock request */ };
//            var response = new ServiceResponse<Product>(
//                mensagemDeErro: "Erro ao atualizar o produto"
//            );

//            _mockService.Setup(service => service.Editar(id, request))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.Put(id, request);

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);
//            var badRequestResult = result as BadRequestObjectResult;
//            Assert.Equal(response.Mensagem, badRequestResult.Value);
//        }


//    }
//}



