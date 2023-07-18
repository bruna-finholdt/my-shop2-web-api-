using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minishop.Controllers;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;
using Moq;
using Xunit;

namespace Minishop.Tests.Controllers
{
    public class SupplierControllerTests
    {
        private readonly Mock<ISupplierService> _mockService;
        private readonly SuppliersController _controller;

        public SupplierControllerTests()
        {
            _mockService = new Mock<ISupplierService>();
            _controller = new SuppliersController(_mockService.Object);
        }

        [Fact]
        public async Task ObterContagem_DeveRetornarOk()
        {
            // Arrange
            var expectedCount = 15;

            _mockService.Setup(service => service.Contar())
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _controller.ObterContagem();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var response = (ItemCountResponse)okResult.Value; // Convert to ItemCountResponse

            Assert.Equal(expectedCount, response.ItemCount);
        }

        [Fact]
        public async Task Get_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 10
            };

            var suppliers = new List<SuppliersResponse>
            {
                new SuppliersResponse(new Supplier
                {
                    Id = 1,
                    CompanyName = "Supplier 1",
                    City = "Uberaba",
                    Uf = "MG",
                    Cnpj = "123456789",
                    Email = "sup1@gmail.com"
                }),

                new SuppliersResponse(new Supplier
                {
                    Id = 2,
                    CompanyName = "Supplier 2",
                    City = "Uberaba",
                    Uf = "MG",
                    Cnpj = "789456123",
                    Email = "sup2@gmail.com"
                }),
            };

            var response = new ServicePagedResponse<SuppliersResponse>(
                suppliers,
                10,
                1,
                10
            );

            _mockService.Setup(service => service.Pesquisar(queryRequest))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Get(queryRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            var resultValue = okResult.Value as ServicePagedResponse<SuppliersResponse>;

            Assert.Equal(response.Conteudo, resultValue.Conteudo);
            Assert.Equal(response.PaginaAtual, resultValue.PaginaAtual);
            Assert.Equal(response.TotalPaginas, resultValue.TotalPaginas);
        }

        [Fact]
        public async Task GetById_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;

            var supplier = new Supplier
            {
                Id = id,
                CompanyName = "Supplier 1",
                City = "Uberaba",
                Uf = "MG",
                Cnpj = "123456789",
                Email = "sup1@gmail.com"
            };

            var expectedResponse = new ServiceResponse<SuppliersCompletoResponse>(new SuppliersCompletoResponse(supplier));

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResponse.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFoundQuandoIdForInvalido()
        {
            // Arrange
            int id = 5;

            string mensagem = "Fornecedor não encontrado";

            var retorno = new ServiceResponse<SuppliersCompletoResponse>(mensagem);

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(retorno);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<SuppliersCompletoResponse>>(notFoundResult.Value);

            Assert.False(response.Sucesso);
            Assert.Equal(mensagem, response.Mensagem);
        }

        [Fact]
        public async Task Post_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            var supplierCreateRequest = new SupplierCreateRequest
            {
                CompanyName = "New Supplier",
                Cnpj = "987654321",
                Email = "new.supplier@example.com",
                City = "New City",
                Uf = "SP",
                Phone = "1234567890",
                ContactName = "Contact Person"
            };

            var supplier = new Supplier
            {
                Id = 1,
                CompanyName = "New Supplier",
                Cnpj = "987654321",
                Email = "new.supplier@example.com",
                City = "New City",
                Uf = "SP",
                Phone = "1234567890",
                ContactName = "Contact Person"
            };

            var expectedResponse = new ServiceResponse<SuppliersResponse>(new SuppliersResponse(supplier));

            _mockService.Setup(service => service.Cadastrar(supplierCreateRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Post(supplierCreateRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResponse.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            var supplierCreateRequest = new SupplierCreateRequest();

            _controller.ModelState.AddModelError("CompanyName", "O nome do fornecedor é obrigatório.");

            // Act
            var result = await _controller.Post(supplierCreateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Put_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;
            var supplierUpdateRequest = new SupplierUpdateRequest
            {
                Email = "updated.supplier@example.com",
                City = "Updated City",
                Uf = "SP",
                Phone = "0987654321",
                ContactName = "Updated Contact Person"
            };

            var supplier = new Supplier
            {
                Id = id,
                CompanyName = "Updated Supplier",
                Cnpj = "987654321",
                Email = "updated.supplier@example.com",
                City = "Updated City",
                Uf = "SP",
                Phone = "0987654321",
                ContactName = "Updated Contact Person"
            };

            var expectedResponse = new ServiceResponse<SuppliersResponse>(new SuppliersResponse(supplier));

            _mockService.Setup(service => service.Editar(id, supplierUpdateRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Put(id, supplierUpdateRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedResponse.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            int id = 1;
            var supplierUpdateRequest = new SupplierUpdateRequest();

            _controller.ModelState.AddModelError("Email", "O e-mail do fornecedor é obrigatório.");

            // Act
            var result = await _controller.Put(id, supplierUpdateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

    }
}

