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
using Microsoft.EntityFrameworkCore;
using k8s.KubeConfigModels;

namespace Minishop.Tests.Services
{
    public class CustomersServiceTests
    {

        private readonly Mock<ICustomersRepository> _mockRepository;
        private readonly CustomersService _customersService;

        public CustomersServiceTests()
        {
            _mockRepository = new Mock<ICustomersRepository>();
            _customersService = new CustomersService(_mockRepository.Object);
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
        public async Task Pesquisar_DeveRetornarItensConsultadosNoBancoDeDados()
        {
            // Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 10
            };

            var customers = new List<Customer>
            {
                new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "1234567890", Cpf = "99988877766" },
                new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Phone = "9876543210", Cpf = "88877766655" },
                new Customer { Id = 3, FirstName = "Michael", LastName = "Johnson", Email = "michael.johnson@example.com", Phone = "5551234567", Cpf = "77766655544" },
                new Customer { Id = 4, FirstName = "Emily", LastName = "Brown", Email = "emily.brown@example.com", Phone = "4449876543", Cpf = "66655544433" },
                new Customer { Id = 5, FirstName = "Daniel", LastName = "Williams", Email = "daniel.williams@example.com", Phone = "1117894562", Cpf = "55544433322" },
                new Customer { Id = 6, FirstName = "Olivia", LastName = "Johnson", Email = "olivia.johnson@example.com", Phone = "2226549871", Cpf = "44433322211" },
                new Customer { Id = 7, FirstName = "James", LastName = "Miller", Email = "james.miller@example.com", Phone = "7776543219", Cpf = "33322211100" },
                new Customer { Id = 8, FirstName = "Sophia", LastName = "Davis", Email = "sophia.davis@example.com", Phone = "8889876545", Cpf = "22211100099" },
                new Customer { Id = 9, FirstName = "Liam", LastName = "Wilson", Email = "liam.wilson@example.com", Phone = "9994561238", Cpf = "11100099988" },
                new Customer { Id = 10, FirstName = "Isabella", LastName = "Anderson", Email = "isabella.anderson@example.com", Phone = "4447894563", Cpf = "00099988877" },
            };

            _mockRepository.Setup(repository => repository.Pesquisar(1, 10)).ReturnsAsync(customers);

            var result = _customersService.Pesquisar(queryRequest).Result;

            Assert.NotNull(result);
            var customersResponse = result.Conteudo.ToList();
            Assert.Equal(queryRequest.Quantidade, customersResponse.Count);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarClienteQuandoIdExistente()
        {
            // Arrange
            int id = 1;
            var existingCustomer = new Customer
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Cpf = "99988877766"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingCustomer);

            // Act
            var response = await _customersService.PesquisaPorId(id);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(existingCustomer.FirstName, response.Conteudo.FirstName);
            Assert.Equal(existingCustomer.LastName, response.Conteudo.LastName);
            Assert.Equal(existingCustomer.Email, response.Conteudo.Email);
            Assert.Equal(existingCustomer.Phone, response.Conteudo.Phone);
            Assert.Equal(existingCustomer.Cpf, response.Conteudo.Cpf);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarErroQuandoClienteNaoEncontrado()
        {
            // Arrange
            int id = 1;
            Customer customer = null;
            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(customer);

            // Act
            var response = await _customersService.PesquisaPorId(id);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("Cliente não encontrado", response.Mensagem);
        }


        [Fact]
        public async Task Cadastrar_DeveSalvarClienteQuandoRequestValido()
        {
            // Arrange
            var request = new CustomerCreateRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Cpf = "99988877766"
            };

            _mockRepository.Setup(repo => repo.VerificarCpfExistente(request.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(repo => repo.VerificarEmailExistente(request.Email)).ReturnsAsync(false);

            Customer savedCustomer = null;
            _mockRepository.Setup(repo => repo.Cadastrar(It.IsAny<Customer>()))
                .Callback<Customer>(customer => savedCustomer = customer)
                .ReturnsAsync((Customer customer) => customer);

            // Act
            var response = await _customersService.Cadastrar(request);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(request.FirstName, savedCustomer.FirstName);
            Assert.Equal(request.LastName, savedCustomer.LastName);
            Assert.Equal(request.Email, savedCustomer.Email);
            Assert.Equal(request.Phone, savedCustomer.Phone);
            Assert.Equal(request.Cpf, savedCustomer.Cpf);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoCpfJaExistente()
        {
            // Arrange
            var request = new CustomerCreateRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Cpf = "11122233344" // CPF já existente no repositório mockado
            };

            _mockRepository.Setup(repo => repo.VerificarCpfExistente(request.Cpf)).ReturnsAsync(true);

            // Act
            var response = await _customersService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("CPF já cadastrado.", response.Mensagem);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoEmailJaExistente()
        {
            // Arrange
            var request = new CustomerCreateRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com", // E-mail já existente no repositório mockado
                Phone = "1234567890",
                Cpf = "99988877766"
            };

            _mockRepository.Setup(repo => repo.VerificarCpfExistente(request.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(repo => repo.VerificarEmailExistente(request.Email)).ReturnsAsync(true);

            // Act
            var response = await _customersService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("E-mail já cadastrado.", response.Mensagem);
        }


        [Fact]
        public async Task Editar_DeveSalvarClienteQuandoRequestValido()
        {
            // Arrange
            int id = 1;
            var request = new CustomerUpdateRequest
            {
                Email = "new-email@example.com",
                Phone = "9876543210",
            };

            var existingCustomer = new Customer
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Cpf = "99988877766"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingCustomer);
            _mockRepository.Setup(repo => repo.VerificarNovoEmailExistente(request.Email, id)).ReturnsAsync(false);

            Customer savedCustomer = null;
            _mockRepository.Setup(repo => repo.Editar(It.IsAny<Customer>()))
                .Callback<Customer>(customer => savedCustomer = customer)
                .ReturnsAsync((Customer customer) => customer);

            // Act
            var response = await _customersService.Editar(id, request);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(request.Email, savedCustomer.Email);
            Assert.Equal(request.Phone, savedCustomer.Phone);
            Assert.Equal(existingCustomer.Cpf, savedCustomer.Cpf); // O CPF não deve ser alterado na edição
        }

        [Fact]
        public async Task Editar_DeveRetornarErroQuandoClienteNaoEncontrado()
        {
            // Arrange
            int id = 1;
            CustomerUpdateRequest request = new CustomerUpdateRequest
            {
                Email = "john.doe@example.com",
                Phone = "1234567890"
            };

            Customer customer = null;
            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(customer);

            // Act
            var response = await _customersService.Editar(id, request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("Cliente não encontrado.", response.Mensagem);
        }

        [Fact]
        public async Task Editar_DeveRetornarErroQuandoEmailJaExistente()
        {
            // Arrange
            int id = 1;
            var request = new CustomerUpdateRequest
            {
                Email = "john.doe@example.com", // E-mail já existente no repositório mockado
                Phone = "1234567890"
            };

            var existingCustomer = new Customer
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com",
                Phone = "9876543210",
                Cpf = "99988877766"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingCustomer);
            _mockRepository.Setup(repo => repo.VerificarNovoEmailExistente(request.Email, id)).ReturnsAsync(true);

            // Act
            var response = await _customersService.Editar(id, request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("E-mail duplicado.", response.Mensagem);
        }

        [Fact]
        public async Task Editar_DeveRetornarErroQuandoCpfNaoEnviado()
        {
            // Arrange
            int id = 1;
            var request = new CustomerUpdateRequest
            {
                Email = "john.doe@example.com",
                Phone = "1234567890"
                // CPF não enviado na request
            };

            var existingCustomer = new Customer
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "9876543210",
                Cpf = "99988877766"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingCustomer);

            // Act
            var response = await _customersService.Editar(id, request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("Erro ao editar cliente.", response.Mensagem);
        }

    }
}



