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
//    public class SupplierControllerTests
//    {
//        private readonly Mock<ISupplierService> _mockService;
//        private readonly SuppliersController _controller;

//        public SupplierControllerTests()
//        {
//            _mockService = new Mock<ISupplierService>();
//            _controller = new SuppliersController(_mockService.Object);
//        }

//        [Fact]
//        public async Task ObterContagem_DeveRetornarOk()
//        {
//            // Arrange
//            var response = 10;
//            _mockService.Setup(service => service.Contar())
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.ObterContagem();

//            // Assert
//            Assert.IsType<OkObjectResult>(result.Result);
//            var okResult = result.Result as OkObjectResult;
//            var itemCountResponse = okResult.Value as ItemCountResponse;
//            Assert.Equal(response, itemCountResponse.ItemCount);
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
//            var response = new ServicePagedResponse<SuppliersResponse>(
//                new SuppliersResponse[] { /* mock response */ },
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
//        public async Task GetById_DeveRetornarOkQuandoServicoRetornarSucesso()
//        {
//            // Arrange
//            int id = 1;
//            var supplier = new Supplier
//            {
//                Id = 1,
//                CompanyName = "Test",
//                ContactName = "Test",
//                Phone = "Test",
//                City = "Test",
//                Uf = "Test",
//                Email = "Test"
//            };
//            var response = new ServiceResponse<SuppliersCompletoResponse>(
//                new SuppliersCompletoResponse(supplier)
//            );

//            _mockService.Setup(service => service.PesquisaPorId(id))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.GetById(id);

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
//            var response = new ServiceResponse<SuppliersCompletoResponse>(
//                mensagemDeErro: "Fornecedor não encontrado"
//            );

//            _mockService.Setup(service => service.PesquisaPorId(id))
//                .ReturnsAsync(response);

//            // Act
//            var result = await _controller.GetById(id);

//            // Assert
//            Assert.IsType<NotFoundObjectResult>(result);
//            var notFoundResult = result as NotFoundObjectResult;
//            Assert.Equal(response, notFoundResult.Value);
//        }

//        [Fact]
//        public async Task Post_DeveRetornarOkQuandoServicoRetornarSucesso()
//        {
//            // Arrange
//            var request = new SupplierCreateRequest { /* mock request */ };
//            var supplier = new Supplier { /* mock supplier */ };
//            var response = new ServiceResponse<SuppliersResponse>(
//                new SuppliersResponse(supplier)
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
//            var request = new SupplierCreateRequest { /* mock invalid request */ };
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
//            var request = new SupplierUpdateRequest { /* mock request */ };
//            var supplier = new Supplier { /* mock supplier */ };
//            var response = new ServiceResponse<Supplier>(
//                new Supplier(/* mock response */)
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
//            var request = new SupplierUpdateRequest { /* mock invalid request */ };
//            _controller.ModelState.AddModelError("PropertyName", "Erro de validação");

//            // Act
//            var result = await _controller.Put(id, request);

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);
//            var badRequestResult = result as BadRequestObjectResult;
//            Assert.IsType<SerializableError>(badRequestResult.Value);
//        }
//    }
//}

