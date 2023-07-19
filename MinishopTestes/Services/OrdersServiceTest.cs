using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minishop.Tests.Services
{
    public class OrdersServiceTest
    {
        private readonly Mock<IOrdersRepository> _mockRepository;
        private readonly OrdersService _ordersService;

        public OrdersServiceTest()
        {
            _mockRepository = new Mock<IOrdersRepository>();
            _ordersService = new OrdersService(_mockRepository.Object);
        }

        [Fact]
        public async Task Contar_DeveRetornarQuantidade()
        {
            // Arrange
            int expectedCount = 15;
            _mockRepository.Setup(repo => repo.Contagem()).ReturnsAsync(expectedCount);

            // Act
            int actualCount = await _ordersService.Contar();

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
                Quantidade = 2
            };

            var customerOrders = new List<CustomerOrder>
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
                        Email = "john.doe@example.com"
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
                        Email = "jane.smith@example.com"
                    },
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Id = 3, OrderId = 2, ProductId = 3, Quantity = 1, UnitPrice = 22.98m }
                    }
                },

            };

            var response = new ServicePagedResponse<CustomerOrder>(
                customerOrders,
                15,
                1,
                10
            );

            _mockRepository.Setup(repository => repository.Pesquisar(queryRequest.PaginaAtual, queryRequest.Quantidade))
                .ReturnsAsync(customerOrders);

            // Act
            var result = await _ordersService.Pesquisar(queryRequest);

            // Assert
            Assert.NotNull(result);
            var customerOrdersResponse = result.Conteudo.ToList();
            Assert.Equal(queryRequest.Quantidade, customerOrdersResponse.Count);

            // Verify each item in the customerOrdersResponse using Assert.All
            Assert.All(customerOrdersResponse, cor =>
            {
                // Verificar se o objeto é do tipo CustomerOrderCompletoResponse
                if (cor is CustomerOrderCompletoResponse corCompleto)
                {
                    var co = customerOrders.SingleOrDefault(co => co.Id == corCompleto.Id);
                    Assert.NotNull(co);

                    Assert.Equal(co.Id, corCompleto.Id);
                    Assert.Equal(co.OrderDate, corCompleto.OrderDate);
                    Assert.Equal(co.TotalAmount, corCompleto.TotalAmount);

                    Assert.Equal(co.Customer.Id, corCompleto.Customer.Id);
                    Assert.Equal(co.Customer.FirstName, corCompleto.Customer.FirstName);
                    Assert.Equal(co.Customer.LastName, corCompleto.Customer.LastName);
                    Assert.Equal(co.Customer.Email, corCompleto.Customer.Email);

                    var coOrderItemsList = co.OrderItems.ToList();
                    var corOrderItemsList = corCompleto.OrderItems.ToList();

                    Assert.Equal(coOrderItemsList.Count, corOrderItemsList.Count);

                    for (int i = 0; i < coOrderItemsList.Count; i++)
                    {
                        Assert.Equal(coOrderItemsList[i].Id, corOrderItemsList[i].Id);
                        Assert.Equal(coOrderItemsList[i].ProductId, corOrderItemsList[i].ProductId);
                        Assert.Equal(coOrderItemsList[i].Quantity, corOrderItemsList[i].Quantity);
                        Assert.Equal(coOrderItemsList[i].UnitPrice, corOrderItemsList[i].UnitPrice);
                        Assert.Equal(coOrderItemsList[i].OrderId, corOrderItemsList[i].OrderId);
                    }
                }
            });
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

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingOrder);

            // Act
            var response = await _ordersService.PesquisaPorId(id);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(existingOrder.Id, response.Conteudo.Id);
            Assert.Equal(existingOrder.OrderDate, response.Conteudo.OrderDate);
            Assert.Equal(existingOrder.TotalAmount, response.Conteudo.TotalAmount);
            Assert.Equal(existingOrder.Customer.Id, response.Conteudo.Customer.Id);
            Assert.Equal(existingOrder.Customer.FirstName, response.Conteudo.Customer.FirstName);
            Assert.Equal(existingOrder.Customer.LastName, response.Conteudo.Customer.LastName);
            Assert.Equal(existingOrder.Customer.Email, response.Conteudo.Customer.Email);

            // Verify OrderItems
            var existingOrderItems = existingOrder.OrderItems.ToList(); // Convert ICollection to List
            Assert.NotNull(response.Conteudo.OrderItems);
            Assert.Equal(existingOrderItems.Count, response.Conteudo.OrderItems.Count);
            for (int i = 0; i < existingOrderItems.Count; i++)
            {
                Assert.Equal(existingOrderItems[i].Id, response.Conteudo.OrderItems[i].Id);
                Assert.Equal(existingOrderItems[i].OrderId, response.Conteudo.OrderItems[i].OrderId);
                Assert.Equal(existingOrderItems[i].ProductId, response.Conteudo.OrderItems[i].ProductId);
                Assert.Equal(existingOrderItems[i].Quantity, response.Conteudo.OrderItems[i].Quantity);
                Assert.Equal(existingOrderItems[i].UnitPrice, response.Conteudo.OrderItems[i].UnitPrice);
            }
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarErroQuandoOrderNaoEncontrada()
        {
            // Arrange
            int idNaoExistente = 100; // Suponha que o ID 100 não existe no banco de dados.
            CustomerOrder orderNaoEncontrada = null;
            _mockRepository.Setup(repo => repo.PesquisaPorId(idNaoExistente)).ReturnsAsync(orderNaoEncontrada);

            // Act
            var response = await _ordersService.PesquisaPorId(idNaoExistente);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("Pedido não encontrado", response.Mensagem);
        }


    }
}
