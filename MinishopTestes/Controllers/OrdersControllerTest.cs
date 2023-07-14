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
    public class OrdersControllerTests
    {
        private readonly Mock<IOrdersService> _mockService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockService = new Mock<IOrdersService>();
            _controller = new OrdersController(_mockService.Object);
        }

        [Fact]
        public async Task ObterContagem_DeveRetornarOk()
        {
            // Arrange
            var response = new ItemCountResponse
            {
                ItemCount = 10
            };

            _mockService.Setup(service => service.Contar())
                .ReturnsAsync(response.ItemCount);

            // Act
            var result = await _controller.ObterContagem();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var actualResponse = okResult.Value as ItemCountResponse;
            Assert.Equal(10, actualResponse.ItemCount);
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
            var response = new ServicePagedResponse<CustomerOrderResponse>(
                new CustomerOrderResponse[] { /* mock response */ },
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
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task Get_DeveRetornarBadRequestQuandoServicoRetornarFalha()
        {
            // Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 10
            };
            var response = new ServicePagedResponse<CustomerOrderResponse>(
                mensagemDeErro: "Erro na pesquisa"
            );

            _mockService.Setup(service => service.Pesquisar(queryRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Get(queryRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(response, badRequestResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;
            var customerOrder = new CustomerOrder
            {
                Id = 1,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100,
                Customer = new Customer { Id = 1 },
                OrderItems = new List<OrderItem> { /* mock order items */ }
            };
            var response = new ServiceResponse<CustomerOrderCompletoResponse>(
                new CustomerOrderCompletoResponse(customerOrder)
            );

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFoundQuandoServicoRetornarFalha()
        {
            // Arrange
            int id = 1;
            var response = new ServiceResponse<CustomerOrderCompletoResponse>(
                mensagemDeErro: "Pedido não encontrado"
            );

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.Equal(response, notFoundResult.Value);
        }
    }
}

