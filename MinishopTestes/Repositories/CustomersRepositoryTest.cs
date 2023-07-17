using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Moq;
//using Minishop.Tests.Helpers;
using System;
using System.Linq;
using Xunit;

namespace Minishop.Tests.Repositories
{
    public class CustomersRepositoryTest : IDisposable
    {
        private readonly Minishop2023Context _dbContext;
        private readonly CustomersRepository _customerRepository;

        public CustomersRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDatabase")
                .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
            _customerRepository = new CustomersRepository(_dbContext);
        }

        [Fact]
        public async Task Contagem_DeveRetornarQuantidadeCorretaDeRegistros()
        {
            // Arrange: Adicionando alguns registros ao banco de dados em memória.
            var customers = new List<Customer>
            {
                new Customer { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "1234567890", Cpf = "111.222.333-44" },
                new Customer { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Phone = "9876543210", Cpf = "444.555.666-77" }
            };

            _dbContext.Customers.AddRange(customers);
            _dbContext.SaveChanges();

            // Act
            var result = await _customerRepository.Contagem();

            // Assert
            Assert.Equal(customers.Count, result); // Verifica se a contagem é igual ao número esperado de registros.
        }


        [Fact]
        public async Task Pesquisar_DeveRetornarListagem()
        {
            int paginaAtual = 1;
            int qtdPagina = 10;

            var customers = new List<Customer>
           {
               new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john_doe@test.com", Phone = "1234567", Cpf = "111.222.333-44" },
               new Customer { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane_doe@test.com", Phone = "1234567", Cpf = "222.333.444-55" }
            };


            _dbContext.Customers.AddRange(customers);
            _dbContext.SaveChanges();

            // Act
            var result = await _customerRepository.Pesquisar(paginaAtual, qtdPagina);

            // Assert
            Assert.Equal(customers.Count, result.Count);
            Assert.Equal("John", result[0].FirstName);
            Assert.Equal("Jane", result[1].FirstName);
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarListaVaziaQuandoNaoHaCustomers()
        {
            // Arrange: Neste cenário, não há nenhum customer no banco de dados.
            // O banco de dados em memória está vazio.

            int paginaAtual = 1;
            int qtdPagina = 10;

            // Act
            var result = await _customerRepository.Pesquisar(paginaAtual, qtdPagina);

            // Assert
            Assert.Empty(result); // Verifica se a lista retornada está vazia.
        }


        [Fact]
        public async Task PesquisaPorId_DeveRetornarCustomerCorreto()
        {
            // Arrange
            var customer1 = new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Cpf = "111.222.333-44"
            };

            var customer2 = new Customer
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "0987654321",
                Cpf = "444.555.666-77"
            };

            _dbContext.Customers.AddRange(customer1, customer2);
            _dbContext.SaveChanges();

            // Act
            var result1 = await _customerRepository.PesquisaPorId(customer1.Id);
            var result2 = await _customerRepository.PesquisaPorId(customer2.Id);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);

            Assert.Equal(customer1.FirstName, result1.FirstName);
            Assert.Equal(customer1.LastName, result1.LastName);
            Assert.Equal(customer1.Email, result1.Email);

            Assert.Equal(customer2.FirstName, result2.FirstName);
            Assert.Equal(customer2.LastName, result2.LastName);
            Assert.Equal(customer2.Email, result2.Email);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarNullQuandoCustomerNaoExiste()
        {
            // Arrange: Neste cenário, não há nenhum customer no banco de dados.
            // O banco de dados em memória está vazio.

            int idNaoExistente = 999; //não existe customer com id 999

            // Act
            var result = await _customerRepository.PesquisaPorId(idNaoExistente);

            // Assert
            Assert.Null(result); // Verifica se o resultado é nulo, indicando que o customer não foi encontrado.
        }

        [Fact]
        public async Task Cadastrar_DeveCriarNovoCustomer()
        {
            // Arrange
            var newCustomer = new Customer
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                Phone = "9876543210",
                Cpf = "888.999.000-11"
            };

            // Act
            var createdCustomer = await _customerRepository.Cadastrar(newCustomer);

            // Assert
            Assert.NotNull(createdCustomer);
            Assert.NotEqual(0, createdCustomer.Id); // Verifica se o Id foi preenchido (não é 0)
            // Verifica se o novo Customer foi adicionado ao contexto (in-memory database)
            Assert.Contains(newCustomer, _dbContext.Customers);
        }


        [Fact]
        public async Task Editar_DeveAtualizarCustomerExistente()
        {
            // Arrange
            var existingCustomer = new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Cpf = "111.222.333-44"
            };

            _dbContext.Customers.Add(existingCustomer);
            _dbContext.SaveChanges();

            // Modifica o e-mail do Customer existente
            existingCustomer.Email = "johnyyyy.doe@example.com";

            // Act
            var updatedCustomer = await _customerRepository.Editar(existingCustomer);

            // Assert
            Assert.NotNull(updatedCustomer);
            Assert.Equal(existingCustomer.Id, updatedCustomer.Id);
            Assert.Equal("johnyyyy.doe@example.com", updatedCustomer.Email);

            // Verifica se o Customer foi atualizado no contexto (in-memory database)
            var customerInContext = _dbContext.Customers.Find(existingCustomer.Id);
            Assert.NotNull(customerInContext);
            Assert.Equal(existingCustomer.Email, customerInContext.Email);
        }



        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}