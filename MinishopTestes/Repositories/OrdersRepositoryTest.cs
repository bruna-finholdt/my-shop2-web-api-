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
    public class OrdersRepositoryTest
    {
        private readonly Minishop2023Context _dbContext;
        private readonly OrdersRepository _ordersRepository;

        public OrdersRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDatabase")
                .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
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
            OrderDate = new DateTime(2023, 7, 1), // Coloque aqui a data do pedido
            TotalAmount = 100.00m, // Coloque aqui o valor total do pedido
            Customer = new Customer // Crie um objeto de CustomerResponse com os dados do cliente
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Cpf = "123456786",
                Email = "john.doe@example.com",
                Phone = "1234567890"
            },
            OrderItems = new List<OrderItem> // Crie uma lista de OrderItems (caso seja necessário)
            {
                new OrderItem
                {
                    Id = 1,
                    // Preencha aqui os dados do OrderItem
                },
                new OrderItem
                {
                    Id = 2,
                    // Preencha aqui os dados do OrderItem
                }
            }
        },
        new CustomerOrder
        {
            Id = 2,
            OrderDate = new DateTime(2023, 7, 2), // Coloque aqui a data do pedido
            TotalAmount = 50.00m, // Coloque aqui o valor total do pedido
            Customer = new Customer // Crie um objeto de CustomerResponse com os dados do cliente
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Cpf = "874564216",
                Email = "jane.smith@example.com",
                Phone = "9876543210"
            },
            OrderItems = new List<OrderItem> // Crie uma lista de OrderItems (caso seja necessário)
            {
                new OrderItem
                {
                    Id = 3,
                    // Preencha aqui os dados do OrderItem
                }
            }
        }
    };

            _dbContext.CustomerOrders.AddRange(orders);
            _dbContext.SaveChanges();

            // Act
            var result = await _ordersRepository.Contagem();

            // Assert
            Assert.Equal(orders.Count, result); // Verifica se a contagem é igual ao número esperado de registros.
        }
    }
}
