using Minishop.DAL.Repositories;
using Minishop.DAL;
using Minishop.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minishop.Tests.Services
{
    public class SuppliersServiceTest
    {
        private readonly Mock<ISuppliersRepository> _mockRepository;
        //private readonly Mock<ICustomersDbContext> _mockContext;
        private readonly SuppliersService _suppliersService;

        public SuppliersServiceTest()
        {
            _mockRepository = new Mock<ISuppliersRepository>();
            //_mockContext = new Mock<ICustomersDbContext>();
            _suppliersService = new SuppliersService(_mockRepository.Object);
        }

        [Fact]
        public async Task Contar_DeveRetornarQuantidade()
        {
            // Arrange
            int expectedCount = 10;
            _mockRepository.Setup(repo => repo.Contagem()).ReturnsAsync(expectedCount);

            // Act
            int actualCount = await _suppliersService.Contar();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
