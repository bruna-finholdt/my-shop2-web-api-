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
using Minishop.Domain.Entity;

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

        [Fact]
        public async Task Pesquisar_DeveRetornarListagemPaginada()
        {
            // Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 2
            };

            var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, CompanyName = "Supplier A" },
                new Supplier { Id = 2, CompanyName = "Supplier B" },

            };

            _mockRepository.Setup(repo => repo.Pesquisar(1, 2)).ReturnsAsync(suppliers);

            // Act
            var result = await _suppliersService.Pesquisar(queryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Conteudo.Count()); // Verifica se a quantidade de fornecedores retornados é a esperada.
                                                      // Verifica se os dados dos fornecedores estão corretos
            var suppliersList = result.Conteudo.ToList();

            Assert.Equal(suppliers[0].Id, suppliersList[0].Id);
            Assert.Equal(suppliers[0].CompanyName, suppliersList[0].CompanyName);
            Assert.Equal(suppliers[0].City, suppliersList[0].City);
            Assert.Equal(suppliers[0].Uf, suppliersList[0].Uf);

            Assert.Equal(suppliers[0].Email, suppliersList[0].Email);

            Assert.Equal(suppliers[1].Id, suppliersList[1].Id);
            Assert.Equal(suppliers[1].CompanyName, suppliersList[1].CompanyName);
            Assert.Equal(suppliers[1].City, suppliersList[1].City);
            Assert.Equal(suppliers[1].Uf, suppliersList[1].Uf);

            Assert.Equal(suppliers[1].Email, suppliersList[1].Email);
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

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(supplier);

            // Act
            var result = await _suppliersService.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(supplier.Id, result.Conteudo.Id);
            Assert.Equal(supplier.CompanyName, result.Conteudo.CompanyName);
            Assert.Equal(supplier.City, result.Conteudo.City);
            Assert.Equal(supplier.Uf, result.Conteudo.Uf);
            Assert.Equal(supplier.Cnpj, result.Conteudo.Cnpj);
            Assert.Equal(supplier.Email, result.Conteudo.Email);
        }

        [Fact]
        public async Task PesquisaPorId_NaoEncontrarSupplier()
        {
            // Arrange
            int id = 99; // Um ID que não existe no banco de dados
            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync((Supplier)null);

            // Act
            var result = await _suppliersService.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Sucesso);
            Assert.Null(result.Conteudo);
            Assert.Equal("Fornecedor não encontrado", result.Mensagem);
        }

        [Fact]
        public async Task Cadastrar_DeveCadastrarFornecedorQuandoRequestValido()
        {
            // Arrange
            var request = new SupplierCreateRequest
            {
                CompanyName = "Supplier New",
                Cnpj = "111222333",
                Email = "supplierNew@example.com",
                City = "City New",
                Uf = "MG",
                Phone = "9876543210",
                ContactName = "Contact New"
            };

            _mockRepository.Setup(repo => repo.VerificarCnpjExistente(request.Cnpj)).ReturnsAsync(false);
            _mockRepository.Setup(repo => repo.VerificarEmailExistente(request.Email)).ReturnsAsync(false);

            var createdSupplier = new Supplier
            {
                Id = 1,
                CompanyName = request.CompanyName,
                Cnpj = request.Cnpj,
                Email = request.Email,
                City = request.City,
                Uf = request.Uf,
                Phone = request.Phone,
                ContactName = request.ContactName
            };

            _mockRepository.Setup(repo => repo.Cadastrar(It.IsAny<Supplier>()))
                .ReturnsAsync((Supplier supplier) => createdSupplier);

            // Act
            var response = await _suppliersService.Cadastrar(request);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(request.CompanyName, response.Conteudo.CompanyName);
            Assert.Equal(request.Cnpj, response.Conteudo.Cnpj);
            Assert.Equal(request.Email, response.Conteudo.Email);
            Assert.Equal(request.City, response.Conteudo.City);
            Assert.Equal(request.Uf, response.Conteudo.Uf);
            Assert.Equal(request.Phone, response.Conteudo.Phone);
            Assert.Equal(request.ContactName, response.Conteudo.ContactName);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoCnpjDuplicado()
        {
            // Arrange
            var request = new SupplierCreateRequest
            {
                CompanyName = "Supplier X",
                Cnpj = "123456789",
                Email = "supplierX@example.com",
                City = "City X",
                Uf = "MG",
                Phone = "9876543210",
                ContactName = "Contact X"
            };

            _mockRepository.Setup(repo => repo.VerificarCnpjExistente(request.Cnpj)).ReturnsAsync(true);

            // Act
            var response = await _suppliersService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("CNPJ duplicado", response.Mensagem);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoEmailDuplicado()
        {
            // Arrange
            var request = new SupplierCreateRequest
            {
                CompanyName = "Supplier Y",
                Cnpj = "987654321",
                Email = "supplierY@example.com",
                City = "City Y",
                Uf = "MG",
                Phone = "9876543210",
                ContactName = "Contact Y"
            };

            _mockRepository.Setup(repo => repo.VerificarCnpjExistente(request.Cnpj)).ReturnsAsync(false);
            _mockRepository.Setup(repo => repo.VerificarEmailExistente(request.Email)).ReturnsAsync(true);

            // Act
            var response = await _suppliersService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("E-mail duplicado", response.Mensagem);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoEstadoInvalido()
        {
            // Arrange
            var request = new SupplierCreateRequest
            {
                CompanyName = "Supplier Z",
                Cnpj = "456789123",
                Email = "supplierZ@example.com",
                City = "City Z",
                Uf = "XX", // Estado inválido
                Phone = "9876543210",
                ContactName = "Contact Z"
            };

            _mockRepository.Setup(repo => repo.VerificarCnpjExistente(request.Cnpj)).ReturnsAsync(false);
            _mockRepository.Setup(repo => repo.VerificarEmailExistente(request.Email)).ReturnsAsync(false);

            // Act
            var response = await _suppliersService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Equal("Estado inválido", response.Mensagem);
        }


    }
}
