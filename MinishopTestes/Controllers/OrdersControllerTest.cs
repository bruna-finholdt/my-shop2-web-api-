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
            var expectedCount = 50;

            _mockService.Setup(service => service.Contar())
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _controller.ObterContagem();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var response = (ItemCountResponse)okResult.Value; // Convert to ItemCountResponse

            Assert.Equal(expectedCount, response.ItemCount);
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

            var customerOrders = new List<CustomerOrderResponse>
        {
            new CustomerOrderResponse(new CustomerOrder
            {
                Id = 1,
                OrderDate = new DateTime(2023, 7, 1),
                TotalAmount = 35.97m,
                Customer = new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com"
                },
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.99m },
                    new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5.99m }
                }
            }),

            new CustomerOrderResponse(new CustomerOrder
            {
                Id = 2,
                OrderDate = new DateTime(2023, 7, 2),
                TotalAmount = 22.98m,
                Customer = new Customer
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com"
                },
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Id = 3, OrderId = 2, ProductId = 3, Quantity = 1, UnitPrice = 22.98m }
                }
            })
        };

            var response = new ServicePagedResponse<CustomerOrderResponse>(
                customerOrders,
                15,
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

            var resultValue = okResult.Value as ServicePagedResponse<CustomerOrderResponse>;

            Assert.Equal(response.Conteudo, resultValue.Conteudo);
            Assert.Equal(response.PaginaAtual, resultValue.PaginaAtual);
            Assert.Equal(response.TotalPaginas, resultValue.TotalPaginas);
        }

        [Fact]
        public async Task GetById_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;

            var customerOrder = new CustomerOrder
            {
                Id = 1,
                OrderDate = new DateTime(2023, 7, 1),
                TotalAmount = 35.97m,
                Customer = new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com"
                },
                OrderItems = new List<OrderItem>
            {
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.99m },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5.99m }
            }
            };

            var expectedResponse = new ServiceResponse<CustomerOrderCompletoResponse>(
                new CustomerOrderCompletoResponse(customerOrder)
            );

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResponse.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFoundQuandoIdNaoForEncontrado()
        {
            // Arrange
            int id = 1;

            var expectedResponse = new ServiceResponse<CustomerOrderCompletoResponse>(
                "Pedido não encontrado"
            );

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<CustomerOrderCompletoResponse>>(notFoundResult.Value);

            Assert.False(response.Sucesso);
            Assert.Equal("Pedido não encontrado", response.Mensagem);
        }


    }
}

