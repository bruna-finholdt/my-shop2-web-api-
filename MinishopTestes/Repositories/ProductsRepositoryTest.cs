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
    public class ProductsRepositoryTest
    {
        private readonly Minishop2023Context _dbContext;
        private readonly ProductsRepository _productsRepository;

        public ProductsRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDatabase")
                .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
            _productsRepository = new ProductsRepository(_dbContext);
        }

        [Fact]
        public async Task Contagem_DeveRetornarQuantidadeCorretaDeRegistros()
        {
            // Arrange: Adicionando alguns registros ao banco de dados em memória.
            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Prod1", SupplierId = 1, UnitPrice = 100, IsDiscontinued = false, PackageName = "1 pacote" },
                 new Product { Id = 2, ProductName = "Prod2", SupplierId = 2, UnitPrice = 50, IsDiscontinued = false, PackageName = "1 pacote" },
            };

            _dbContext.Products.AddRange(products);
            _dbContext.SaveChanges();

            // Act
            var result = await _productsRepository.Contagem();

            // Assert
            Assert.Equal(products.Count, result); // Verifica se a contagem é igual ao número esperado de registros.
        }
    }
}
