using Minishop.DAL.Repositories;
using Minishop.DAL;
using Minishop.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minishop.Domain.DTO;

namespace Minishop.Tests.Services
{
    public class ProductsServiceTest
    {
        private readonly Mock<IProductsRepository> _mockRepository;
        //private readonly Mock<ICustomersDbContext> _mockContext;
        private readonly ProductsService _productsService;

        public ProductsServiceTest()
        {
            _mockRepository = new Mock<IProductsRepository>();
            //_mockContext = new Mock<ICustomersDbContext>();
            _productsService = new ProductsService(_mockRepository.Object);
        }

        [Fact]
        public async Task Contar_DeveRetornarQuantidade()
        {
            // Arrange
            int expectedTotalCount = 20;
            int expectedActiveCount = 10;
            int expectedInactiveCount = 5;

            _mockRepository.Setup(repo => repo.Contagem()).ReturnsAsync(expectedTotalCount);
            _mockRepository.Setup(repo => repo.ContagemProdutosAtivos()).ReturnsAsync(expectedActiveCount);
            _mockRepository.Setup(repo => repo.ContagemProdutosInativos()).ReturnsAsync(expectedInactiveCount);

            // Act
            ProductCountResponse response = await _productsService.Contar();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedTotalCount, response.ItemCount);
            Assert.Equal(expectedActiveCount, response.ActiveCount);
            Assert.Equal(expectedInactiveCount, response.InactiveCount);
        }


    }
}
