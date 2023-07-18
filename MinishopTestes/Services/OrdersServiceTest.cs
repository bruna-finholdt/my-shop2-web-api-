using Minishop.DAL.Repositories;
using Minishop.Services;
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
    }
}
