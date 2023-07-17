using System.Threading.Tasks;
using Castle.Core.Resource;
using Humanizer;
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
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomersService> _mockService;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockService = new Mock<ICustomersService>();
            _controller = new CustomersController(_mockService.Object);
        }

        [Fact]
        public async Task ObterContagem_DeveRetornarOk()
        {
            // Arrange
            var expectedCount = 10;

            _mockService.Setup(service => service.Contar())
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _controller.ObterContagem();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var response = (ItemCountResponse)okResult.Value; // Conversão explícita 
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

            var customers = new List<CustomerResponse>
            {
                new CustomerResponse(new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "1234567890"
                }),

                new CustomerResponse(new Customer
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Phone = "9876543210"
                }),
            };

            var response = new ServicePagedResponse<CustomerResponse>(
                customers,
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

            var resultValue = okResult.Value as ServicePagedResponse<CustomerResponse>;

            Assert.Equal(response.Conteudo, resultValue.Conteudo);
            Assert.Equal(response.PaginaAtual, resultValue.PaginaAtual);
            Assert.Equal(response.TotalPaginas, resultValue.TotalPaginas);
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
            var response = new ServicePagedResponse<CustomerResponse>(
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

            var customer = new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
            };

            var orderItem1 = new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.99m };
            var orderItem2 = new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5.99m };

            var customerOrder = new CustomerOrder
            {
                Id = 1,
                OrderDate = new DateTime(2023, 7, 1),
                CustomerId = 1,
                TotalAmount = 35.97m,
                Customer = customer,
                OrderItems = new List<OrderItem> { orderItem1, orderItem2 }
            };

            var expectedCustomer = new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                CustomerOrders = new List<CustomerOrder> { customerOrder },
            };

            var response = new ServiceResponse<CustomerCompletoResponse>(
                new CustomerCompletoResponse(expectedCustomer)
            );

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id);

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
            var response = new ServiceResponse<CustomerCompletoResponse>(
                mensagemDeErro: "Cliente não encontrado"
            );

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.Equal(response, notFoundResult.Value);
        }

        [Fact]
        public async Task Post_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            var request = new CustomerCreateRequest
            {
                Cpf = "12345678",
                Email = "john.doe@test.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "123456789"
            };
            var customer = new Customer
            {
                Cpf = "12345678",
                Email = "john.doe@test.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "123456789"
            };
            var response = new ServiceResponse<CustomerResponse>(
                new CustomerResponse(customer)
            );

            _mockService.Setup(service => service.Cadastrar(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Post(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            var request = new CustomerCreateRequest
            {
                Cpf = "12345678",
                Email = "john.doe@test.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "123456789"
            };
            _controller.ModelState.AddModelError("PropertyName", "Erro de validação");

            // Act
            var result = await _controller.Post(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Put_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;
            var request = new CustomerUpdateRequest
            {
                Phone = "23456"
            };

            var customer = new Customer
            {
                Cpf = "12345678",
                Email = "john.doe@test.com",
                FirstName = "John",
                LastName = "Doe",
                Phone = "123456789"
            };

            var response = new ServiceResponse<Customer>(
                new Customer
                {
                    Cpf = "12345678",
                    Email = "john.doe@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "23456"
                }
            );

            _mockService.Setup(service => service.Editar(id, request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Put(id, request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            int id = 1;
            var request = new CustomerUpdateRequest
            {
                Phone = "23456"
            };

            _controller.ModelState.AddModelError("PropertyName", "Erro de validação");

            // Act
            var result = await _controller.Put(id, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequestQuandoServicoRetornarFalha()
        {
            // Arrange
            int id = 1;
            var request = new CustomerUpdateRequest
            {
                Phone = "23456"
            };
            var response = new ServiceResponse<Customer>(
                mensagemDeErro: "Erro ao atualizar o cliente"
            );

            _mockService.Setup(service => service.Editar(id, request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Put(id, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(response.Mensagem, badRequestResult.Value);
        }
    }
}

