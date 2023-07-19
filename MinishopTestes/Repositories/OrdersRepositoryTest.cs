using Castle.Core.Resource;
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
    public class OrdersRepositoryTest : IDisposable
    {
        private readonly Minishop2023Context _dbContext;
        private readonly OrdersRepository _ordersRepository;

        public OrdersRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                .UseInMemoryDatabase(databaseName: "TestOrderDatabase")
                .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
            _dbContext.Database.EnsureDeleted(); // Clear the database before each test
            _ordersRepository = new OrdersRepository(_dbContext);
        }

        [Fact]
        public async Task Contagem_DeveRetornarQuantidadeCorretaDeRegistros()
        {
            // Arrange: Adicionando alguns registros ao banco de dados em memória.
            var orders = new List<CustomerOrder>
            {
                new CustomerOrder
                {
                    Id = 1,
                    OrderDate = new DateTime(2023, 7, 1),
                    TotalAmount = 35.97m,
                    Customer = new Customer
                    {
                        Id = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@example.com",
                        Cpf = "12345678"
                    },
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.99m },
                        new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5.99m }
                    }
                },
                new CustomerOrder
                {
                    Id = 2,
                    OrderDate = new DateTime(2023, 7, 2),
                    TotalAmount = 22.98m,
                    Customer = new Customer
                    {
                        Id = 2,
                        FirstName = "Jane",
                        LastName = "Smith",
                        Email = "jane.smith@example.com",
                        Cpf = "897542657"
                    },
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Id = 3, OrderId = 2, ProductId = 3, Quantity = 1, UnitPrice = 22.98m }
                    }
                },

            };

            _dbContext.CustomerOrders.AddRange(orders);
            _dbContext.SaveChanges();

            // Act
            var result = await _ordersRepository.Contagem();

            // Assert
            Assert.Equal(orders.Count, result); // Verifica se a contagem é igual ao número esperado de registros.
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarListagem()
        {
            // Arrange
            int paginaAtual = 1;
            int qtdPagina = 2;

            // Criar novos objetos Customer
            var customers = new List<Customer>
    {
        new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john_doe@test.com", Phone = "1234567", Cpf = "111.222.333-44" },
        new Customer { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane_doe@test.com", Phone = "1234567", Cpf = "222.333.444-55" }
    };

            // Criar novos objetos CustomerOrder, associando cada Customer a um CustomerOrder
            var customerOrders = new List<CustomerOrder>
    {
        new CustomerOrder
        {
            Id = 1,
            OrderDate = new DateTime(2023, 7, 1),
            TotalAmount = 100.00m,
            Customer = customers[0], // Usar o mesmo objeto Customer para o primeiro pedido
            OrderItems = new List<OrderItem>
            {
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.99m },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5.99m }
            }
        },
        new CustomerOrder
        {
            Id = 2,
            OrderDate = new DateTime(2023, 7, 2),
            TotalAmount = 50.00m,
            Customer = customers[1], // Usar o mesmo objeto Customer para o segundo pedido
            OrderItems = new List<OrderItem>
            {
                new OrderItem { Id = 3, OrderId = 2, ProductId = 3, Quantity = 1, UnitPrice = 22.98m }
            }
        }
    };

            _dbContext.Customers.AddRange(customers);
            _dbContext.CustomerOrders.AddRange(customerOrders);
            _dbContext.SaveChanges();

            // Act
            var result = await _ordersRepository.Pesquisar(paginaAtual, qtdPagina);

            // Assert
            Assert.Equal(qtdPagina, result.Count);
            Assert.Equal(customerOrders[0].Id, result[0].Id);
            Assert.Equal(customerOrders[0].OrderDate, result[0].OrderDate);
            Assert.Equal(customerOrders[0].TotalAmount, result[0].TotalAmount);
            Assert.Equal(customers[0].Id, result[0].Customer.Id);
            Assert.Equal(customers[0].FirstName, result[0].Customer.FirstName);
            Assert.Equal(customers[0].LastName, result[0].Customer.LastName);
            Assert.Equal(customers[0].Email, result[0].Customer.Email);

            Assert.Equal(customerOrders[1].Id, result[1].Id);
            Assert.Equal(customerOrders[1].OrderDate, result[1].OrderDate);
            Assert.Equal(customerOrders[1].TotalAmount, result[1].TotalAmount);
            Assert.Equal(customers[1].Id, result[1].Customer.Id);
            Assert.Equal(customers[1].FirstName, result[1].Customer.FirstName);
            Assert.Equal(customers[1].LastName, result[1].Customer.LastName);
            Assert.Equal(customers[1].Email, result[1].Customer.Email);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarCustomerOrderQuandoIdExistente()
        {
            // Arrange
            int id = 1;
            var existingOrder = new CustomerOrder
            {
                Id = id,
                OrderDate = new DateTime(2023, 7, 1),
                TotalAmount = 35.97m,
                Customer = new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Cpf = "12345678"
                },
                OrderItems = new List<OrderItem>
        {
            new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.99m },
            new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5.99m }
        }
            };

            _dbContext.CustomerOrders.Add(existingOrder);
            _dbContext.SaveChanges();

            // Act
            var result = await _ordersRepository.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingOrder.Id, result.Id);
            Assert.Equal(existingOrder.OrderDate, result.OrderDate);
            Assert.Equal(existingOrder.TotalAmount, result.TotalAmount);
            Assert.Equal(existingOrder.Customer.Id, result.Customer.Id);
            Assert.Equal(existingOrder.Customer.FirstName, result.Customer.FirstName);
            Assert.Equal(existingOrder.Customer.LastName, result.Customer.LastName);
            Assert.Equal(existingOrder.Customer.Email, result.Customer.Email);

            // Convert ICollection<OrderItem> to List<OrderItem>
            var existingOrderItems = existingOrder.OrderItems.ToList();
            Assert.NotNull(result.OrderItems);
            Assert.Equal(existingOrderItems.Count, result.OrderItems.Count);

            // Verify OrderItems
            var resultOrderItems = result.OrderItems.ToList(); // Convert ICollection<OrderItemResponse> to List<OrderItemResponse>
            for (int i = 0; i < existingOrderItems.Count; i++)
            {
                Assert.Equal(existingOrderItems[i].Id, resultOrderItems[i].Id);
                Assert.Equal(existingOrderItems[i].OrderId, resultOrderItems[i].OrderId);
                Assert.Equal(existingOrderItems[i].ProductId, resultOrderItems[i].ProductId);
                Assert.Equal(existingOrderItems[i].Quantity, resultOrderItems[i].Quantity);
                Assert.Equal(existingOrderItems[i].UnitPrice, resultOrderItems[i].UnitPrice);
            }
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarNullQuandoOrderNaoExiste()
        {
            // Arrange: // Arrange
            var order = new CustomerOrder
            {
                Id = 1,
                OrderDate = new DateTime(2023, 7, 1),
                TotalAmount = 35.97m,
                Customer = new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Cpf = "12345678"
                },
                OrderItems = new List<OrderItem>
        {
            new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.99m },
            new OrderItem { Id = 2, OrderId = 1, ProductId = 2, Quantity = 3, UnitPrice = 5.99m }
        }
            };

            _dbContext.CustomerOrders.AddRange(order);
            _dbContext.SaveChanges();

            int idNaoExistente = 5;

            // Act
            var result = await _ordersRepository.PesquisaPorId(idNaoExistente);

            // Assert
            Assert.Null(result); // Verifica se o resultado é nulo, indicando que o pedido não foi encontrado.
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
