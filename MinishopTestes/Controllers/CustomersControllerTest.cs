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
            var actualCount = (ItemCountResponse)okResult.Value; // Conversão explícita 
            Assert.Equal(expectedCount, actualCount.ItemCount);
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
            var response = new ServicePagedResponse<CustomerResponse>(
                new CustomerResponse[] { /* mock response */ },
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
                Phone = "1234567890"
            };
            var response = new ServiceResponse<CustomerCompletoResponse>(
                new CustomerCompletoResponse(customer)
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
            var request = new CustomerCreateRequest { /* mock request */ };
            var customer = new Customer { /* mock customer */ };
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
            var request = new CustomerCreateRequest { /* mock invalid request */ };
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
            var request = new CustomerUpdateRequest { /* mock request */ };
            var customer = new Customer { /* mock customer */ };
            var response = new ServiceResponse<Customer>(
                new Customer(/* mock response */)
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
            var request = new CustomerUpdateRequest { /* mock invalid request */ };
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
            var request = new CustomerUpdateRequest { /* mock request */ };
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

