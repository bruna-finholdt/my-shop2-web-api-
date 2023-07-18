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
    public class SuppliersRepositoryTest
    {
        private readonly Minishop2023Context _dbContext;
        private readonly SuppliersRepository _suppliersRepository;

        public SuppliersRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDatabase")
                .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
            _suppliersRepository = new SuppliersRepository(_dbContext);
        }

        [Fact]
        public async Task Contagem_DeveRetornarQuantidadeCorretaDeRegistros()
        {
            // Arrange: Adicionando alguns registros ao banco de dados em memória.
            var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, CompanyName = "Supplier1", City = "Uberaba", Uf = "MG", Cnpj = "123456789", Email = "sup1@gmail.com" },
                new Supplier { Id = 2, CompanyName = "Supplier2", City = "Uberaba", Uf = "MG", Cnpj = "789456123", Email = "sup2@gmail.com" },
            };

            _dbContext.Suppliers.AddRange(suppliers);
            _dbContext.SaveChanges();

            // Act
            var result = await _suppliersRepository.Contagem();

            // Assert
            Assert.Equal(suppliers.Count, result); // Verifica se a contagem é igual ao número esperado de registros.
        }
    }
}



