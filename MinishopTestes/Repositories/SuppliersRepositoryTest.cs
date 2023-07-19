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
    public class SuppliersRepositoryTest : IDisposable
    {
        private readonly Minishop2023Context _dbContext;
        private readonly SuppliersRepository _suppliersRepository;

        public SuppliersRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                .UseInMemoryDatabase(databaseName: "TestSupplierDatabase")
                .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
            _dbContext.Database.EnsureDeleted(); // Clear the database before each test
            _suppliersRepository = new SuppliersRepository(_dbContext);
        }

        [Fact]
        public async Task Contagem_DeveRetornarQuantidadeCorretaDeRegistros()
        {
            // Arrange: Adicionando alguns registros ao banco de dados em memória.
            var suppliers = new List<Supplier>
        {
                    new Supplier {  CompanyName = "Supplier A", City = "City A", Uf = "MG", Cnpj = "123456789", Email = "supplierA@example.com" },
                    new Supplier {  CompanyName = "Supplier B", City = "City B", Uf = "MG", Cnpj = "987654321", Email = "supplierB@example.com" },
                    new Supplier {  CompanyName = "Supplier C", City = "City C", Uf = "MG", Cnpj = "456789123", Email = "supplierC@example.com" },
                    new Supplier {  CompanyName = "Supplier D", City = "City D", Uf = "MG", Cnpj = "789123456", Email = "supplierD@example.com" }
        };

            _dbContext.Suppliers.AddRange(suppliers);
            _dbContext.SaveChanges();

            // Act
            var result = await _suppliersRepository.Contagem();

            // Assert
            Assert.Equal(suppliers.Count, result); // Verifica se a contagem é igual ao número esperado de registros.
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarListagemPaginada()
        {
            // Arrange
            int paginaAtual = 1;
            int qtdPagina = 2;

            var suppliers = new List<Supplier>
                {
                    new Supplier { Id = 1, CompanyName = "Supplier A", City = "City A", Uf = "MG", Cnpj = "123456789", Email = "supplierA@example.com" },
                    new Supplier { Id = 2, CompanyName = "Supplier B", City = "City B", Uf = "MG", Cnpj = "987654321", Email = "supplierB@example.com" },
                    new Supplier { Id = 3, CompanyName = "Supplier C", City = "City C", Uf = "MG", Cnpj = "456789123", Email = "supplierC@example.com" },
                    new Supplier { Id = 4, CompanyName = "Supplier D", City = "City D", Uf = "MG", Cnpj = "789123456", Email = "supplierD@example.com" }
                };

            _dbContext.Suppliers.AddRange(suppliers);
            _dbContext.SaveChanges();

            // Act
            var result = await _suppliersRepository.Pesquisar(paginaAtual, qtdPagina);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(qtdPagina, result.Count); // Verifica se a quantidade de fornecedores retornados é a esperada.
                                                   // Verifica se os dados dos fornecedores estão corretos
            Assert.Equal(suppliers[0].Id, result[0].Id);
            Assert.Equal(suppliers[0].CompanyName, result[0].CompanyName);
            Assert.Equal(suppliers[0].City, result[0].City);
            Assert.Equal(suppliers[0].Uf, result[0].Uf);
            Assert.Equal(suppliers[0].Cnpj, result[0].Cnpj);
            Assert.Equal(suppliers[0].Email, result[0].Email);

            Assert.Equal(suppliers[1].Id, result[1].Id);
            Assert.Equal(suppliers[1].CompanyName, result[1].CompanyName);
            Assert.Equal(suppliers[1].City, result[1].City);
            Assert.Equal(suppliers[1].Uf, result[1].Uf);
            Assert.Equal(suppliers[1].Cnpj, result[1].Cnpj);
            Assert.Equal(suppliers[1].Email, result[1].Email);
        }

        [Fact]
        public async Task PesquisaPorId_EncontrarSupplier()
        {
            // Arrange
            int id = 1;
            var supplier = new Supplier
            {
                Id = 1,
                CompanyName = "Supplier A",
                City = "City A",
                Uf = "MG",
                Cnpj = "123456789",
                Email = "supplierA@example.com"
            };

            _dbContext.Suppliers.Add(supplier);
            _dbContext.SaveChanges();

            // Act
            var result = await _suppliersRepository.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(supplier.Id, result.Id);
            Assert.Equal(supplier.CompanyName, result.CompanyName);
            Assert.Equal(supplier.City, result.City);
            Assert.Equal(supplier.Uf, result.Uf);
            Assert.Equal(supplier.Cnpj, result.Cnpj);
            Assert.Equal(supplier.Email, result.Email);
        }

        [Fact]
        public async Task PesquisaPorId_NaoEncontrarSupplier()
        {
            // Arrange
            var supplier = new Supplier
            {
                Id = 1,
                CompanyName = "Supplier A",
                City = "City A",
                Uf = "MG",
                Cnpj = "123456789",
                Email = "supplierA@example.com"
            };

            _dbContext.Suppliers.Add(supplier);
            _dbContext.SaveChanges();

            int idNaoExistente = 5;

            //Act
            var result = await _suppliersRepository.PesquisaPorId(idNaoExistente);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarNovoFornecedorQuandoRequestValido()
        {
            // Arrange
            var supplier = new Supplier
            {
                CompanyName = "Supplier New",
                Cnpj = "111222333",
                Email = "supplierNew@example.com",
                City = "City New",
                Uf = "MG",
                Phone = "9876543210",
                ContactName = "Contact New"
            };

            // Act
            var result = await _suppliersRepository.Cadastrar(supplier);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id); // Verifica se o Id do fornecedor foi gerado corretamente
            Assert.Equal(supplier.CompanyName, result.CompanyName);
            Assert.Equal(supplier.Cnpj, result.Cnpj);
            Assert.Equal(supplier.Email, result.Email);
            Assert.Equal(supplier.City, result.City);
            Assert.Equal(supplier.Uf, result.Uf);
            Assert.Equal(supplier.Phone, result.Phone);
            Assert.Equal(supplier.ContactName, result.ContactName);
        }

        [Fact]
        public async Task Editar_DeveRetornarFornecedorAtualizadoQuandoRequestValido()
        {
            // Arrange
            var existingSupplier = new Supplier
            {
                Id = 1,
                CompanyName = "Supplier A",
                Cnpj = "123456789",
                Email = "supplierOld@example.com",
                City = "City Old",
                Uf = "MG",
                Phone = "9876543210",
                ContactName = "Contact Old"
            };

            _dbContext.Suppliers.Add(existingSupplier);
            _dbContext.SaveChanges();

            // Modifica os dados do Supplier existente
            existingSupplier.Uf = "SP";
            existingSupplier.City = "São Paulo";

            // Act
            var updatedSupplier = await _suppliersRepository.Editar(existingSupplier);

            // Assert
            Assert.NotNull(updatedSupplier);
            Assert.Equal(existingSupplier.Id, updatedSupplier.Id);
            Assert.Equal("SP", updatedSupplier.Uf);
            Assert.Equal("São Paulo", updatedSupplier.City);

            // Verifica se o Supplier foi atualizado no contexto (banco de dados em memória)
            var supplierInContext = _dbContext.Suppliers.Find(existingSupplier.Id);
            Assert.NotNull(supplierInContext);
            Assert.Equal("SP", updatedSupplier.Uf);
            Assert.Equal("São Paulo", updatedSupplier.City);
        }

        // Implement the Dispose method to clean up resources after the test.
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}



