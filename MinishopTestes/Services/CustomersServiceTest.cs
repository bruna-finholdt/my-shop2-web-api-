using System.Threading.Tasks;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Castle.Core.Resource;


namespace Minishop.Tests.Services
{
    public class CustomersServiceTests
    {

        private readonly Mock<ICustomersRepository> _mockRepository;
        private readonly Mock<ICustomersDbContext> _mockContext;
        private readonly CustomersService _customersService;

        public CustomersServiceTests()
        {
            _mockRepository = new Mock<ICustomersRepository>();
            _mockContext = new Mock<ICustomersDbContext>();
            _customersService = new CustomersService(_mockRepository.Object, _mockContext.Object);
        }

        [Fact]
        public async Task Contar_DeveRetornarQuantidade()
        {
            // Arrange
            int expectedCount = 10;
            _mockRepository.Setup(repo => repo.Contagem()).ReturnsAsync(expectedCount);

            // Act
            int actualCount = await _customersService.Contar();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarServicePagedResponseComListaDeClientes()
        {
            // Arrange
            int paginaAtual = 1;
            int quantidadePorPagina = 10;
            int totalClientes = 20;

            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = paginaAtual,
                Quantidade = quantidadePorPagina
            };

            var customers = new List<Customer>
            {
                new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "1234567890" },
                new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Phone = "1234567890" }
            };

            var response = new ServicePagedResponse<CustomerResponse>(
                customers.Select(customer => new CustomerResponse(customer)).ToList(),
                totalClientes,
                paginaAtual,
                quantidadePorPagina
            );

            _mockRepository.Setup(repo => repo.Pesquisar(paginaAtual, quantidadePorPagina))
                .ReturnsAsync(customers);

            _mockRepository.Setup(repo => repo.Contagem())
                .ReturnsAsync(totalClientes);

            // Act
            var result = await _customersService.Pesquisar(queryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(response.Conteudo, result.Conteudo);
            Assert.Equal(response.PaginaAtual, result.PaginaAtual);
            Assert.Equal(response.TotalPaginas, result.TotalPaginas);
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarServicePagedResponseVazioQuandoCustomersRepositoryRetornaListaVazia()
        {
            // Arrange
            int paginaAtual = 1;
            int quantidadePorPagina = 10;
            int totalClientes = 0; // Simulando lista vazia
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = paginaAtual,
                Quantidade = quantidadePorPagina
            };

            var customers = new List<Customer>(); // Lista vazia

            var response = new ServicePagedResponse<CustomerResponse>(
                customers.Select(customer => new CustomerResponse(customer)).ToList(),
                totalClientes,
                paginaAtual,
                quantidadePorPagina
            );

            _mockRepository.Setup(repo => repo.Pesquisar(paginaAtual, quantidadePorPagina))
                .ReturnsAsync(customers);

            _mockRepository.Setup(repo => repo.Contagem())
                .ReturnsAsync(totalClientes);

            // Act
            var result = await _customersService.Pesquisar(queryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Conteudo); // Verifica se a lista de conteúdo está vazia
            Assert.Equal(paginaAtual, result.PaginaAtual);
            Assert.Equal(0, result.TotalPaginas); // Verifica se o número total de páginas é 0
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarServiceResponseComClienteCompleto()
        {
            // Arrange
            int id = 1;

            var customer = new Customer
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
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

            var response = new ServiceResponse<CustomerCompletoResponse>(new CustomerCompletoResponse(expectedCustomer));

            _mockRepository.Setup(repo => repo.PesquisaPorId(id))
                .ReturnsAsync(expectedCustomer);

            // Act
            var result = await _customersService.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Sucesso);
            Assert.NotNull(result.Conteudo);
            Assert.Equivalent(response.Conteudo, result.Conteudo);

        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarServiceResponseComErroQuandoClienteNaoEncontrado()
        {
            // Arrange
            int id = 1;

            _mockRepository.Setup(repo => repo.PesquisaPorId(id))
                .ReturnsAsync((Customer)null); // Configura o retorno como null para simular cliente não encontrado

            // Act
            var result = await _customersService.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Sucesso);
            Assert.Null(result.Conteudo);
            Assert.Equal("Cliente não encontrado", result.Mensagem);
        }







    }
}



