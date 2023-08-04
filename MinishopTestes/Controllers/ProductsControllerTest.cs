using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minishop.Controllers;
using Minishop.DAL;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;
using Moq;
using Xunit;

namespace Minishop.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductsService> _mockService;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductsService>();
            _mockStorageService = new Mock<IStorageService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [Fact]
        public async Task ObterContagem_DeveRetornarOk()
        {
            // Arrange
            var expectedTotalCount = 100;
            var expectedActiveCount = 80;
            var expectedInactiveCount = 20;

            var productCountResponse = new ProductCountResponse
            {
                ItemCount = expectedTotalCount,
                ActiveCount = expectedActiveCount,
                InactiveCount = expectedInactiveCount
            };

            _mockService.Setup(service => service.Contar())
                .ReturnsAsync(productCountResponse);

            // Act
            var result = await _controller.ObterContagem();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var response = (ProductCountResponse)okResult.Value; // Convert to ProductCountResponse

            // Check if the response values match the expected counts
            Assert.Equal(expectedTotalCount, response.ItemCount);
            Assert.Equal(expectedActiveCount, response.ActiveCount);
            Assert.Equal(expectedInactiveCount, response.InactiveCount);
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

            var products = new List<ProductsResponse>
    {
        new ProductsResponse(new Product
        {
            Id = 1,
            ProductName = "Product 1",
            SupplierId = 1,
            UnitPrice = 10.99m,
            IsDiscontinued = false,
            PackageName = "Package 1"
        }),

        new ProductsResponse(new Product
        {
            Id = 2,
            ProductName = "Product 2",
            SupplierId = 2,
            UnitPrice = 5.99m,
            IsDiscontinued = true,
            PackageName = "Package 2"
        }),
    };

            var response = new ServicePagedResponse<ProductsResponse>(
                products,
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

            var resultValue = okResult.Value as ServicePagedResponse<ProductsResponse>;

            Assert.Equal(response.Conteudo, resultValue.Conteudo);
            Assert.Equal(response.PaginaAtual, resultValue.PaginaAtual);
            Assert.Equal(response.TotalPaginas, resultValue.TotalPaginas);
        }

        [Fact]
        public async Task GetById_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;

            var product = new Product
            {
                Id = id,
                ProductName = "Product 1",
                SupplierId = 1,
                UnitPrice = 10.99m,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            var response = new ServiceResponse<ProductsCompletoResponse>(new ProductsCompletoResponse(product));

            _mockService.Setup(service => service.PesquisaPorIdCompleto(id))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFoundQuandoIdForInvalido()
        {
            // Arrange
            int id = 5;

            string mensagem = "Produto não encontrado";

            var retorno = new ServiceResponse<ProductsCompletoResponse>(mensagem);

            _mockService.Setup(service => service.PesquisaPorIdCompleto(id))
                .ReturnsAsync(retorno);

            // Act
            var result = await _controller.GetById(id.ToString());

            // Assert
            var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<ProductsCompletoResponse>>(NotFoundResult.Value);

            Assert.False(response.Sucesso);
            Assert.Equal(mensagem, response.Mensagem);
        }

        [Fact]
        public async Task Post_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            var productCreateRequest = new ProductCreateRequest
            {
                ProductName = "New Product",
                SupplierId = 1,
                UnitPrice = 19.99m,
                IsDiscontinued = false,
                PackageName = "New Package"
            };

            var product = new Product
            {
                Id = 1,
                ProductName = "New Product",
                SupplierId = 1,
                UnitPrice = 19.99m,
                IsDiscontinued = false,
                PackageName = "New Package"
            };

            var expectedResponse = new ServiceResponse<ProductsCompletoResponse>(new ProductsCompletoResponse(product));

            _mockService.Setup(service => service.Cadastrar(productCreateRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Post(productCreateRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ServiceResponse<ProductsCompletoResponse>; // Extrair o ServiceResponse<ProductsCompletoResponse>
            Assert.Equal(expectedResponse.Conteudo, response.Conteudo); // Comparar apenas a propriedade Conteudo
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            var productCreateRequest = new ProductCreateRequest
            {
                // Deixando o ProductName vazio para simular um modelo inválido
                ProductName = "",
                SupplierId = 1,
                UnitPrice = 19.99m,
                IsDiscontinued = false,
                PackageName = "New Package"
            };

            _controller.ModelState.AddModelError("ProductName", "O nome do produto é obrigatório.");

            // Act
            var result = await _controller.Post(productCreateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);

            var error = badRequestResult.Value as SerializableError;
            Assert.True(error.ContainsKey("ProductName"));
        }

        [Fact]
        public async Task Put_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int id = 1;

            var productUpdateRequest = new ProductUpdateRequest
            {
                ProductName = "Updated Product",
                SupplierId = 2,
                UnitPrice = 29.99m,
                IsDiscontinued = true,
                PackageName = "Updated Package"
            };

            var product = new Product
            {
                Id = id,
                ProductName = "Updated Product",
                SupplierId = 2,
                UnitPrice = 29.99m,
                IsDiscontinued = true,
                PackageName = "Updated Package"
            };

            var expectedResponse = new ServiceResponse<ProductsCompletoResponse>(new ProductsCompletoResponse(product));

            _mockService.Setup(service => service.Editar(id, productUpdateRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Put(id, productUpdateRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ServiceResponse<ProductsCompletoResponse>; // Extrair o ServiceResponse<ProductsCompletoResponse>
            Assert.Equal(expectedResponse.Conteudo, response.Conteudo); // Comparar apenas a propriedade Conteudo
        }


        [Fact]
        public async Task Put_DeveRetornarBadRequestQuandoModelInvalido()
        {
            // Arrange
            int id = 1;

            var productUpdateRequest = new ProductUpdateRequest
            {
                // Deixando o ProductName vazio para simular um modelo inválido
                ProductName = "",
                SupplierId = 2,
                UnitPrice = 29.99m,
                IsDiscontinued = true,
                PackageName = "Updated Package"
            };

            _controller.ModelState.AddModelError("ProductName", "O nome do produto é obrigatório.");

            // Act
            var result = await _controller.Put(id, productUpdateRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<SerializableError>(badRequestResult.Value);

            var error = badRequestResult.Value as SerializableError;
            Assert.True(error.ContainsKey("ProductName"));
        }

        [Fact]
        public async Task Post_ImagemDoProduto_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            //Arrange
            int productId = 1;
            var formFile = new FormFile(new MemoryStream(new byte[0]), 0, 0, "testFile", "test.png");//fakefile
            var files = new List<IFormFile> { formFile };

            //Simula o serviço de armazenamento
            string imageUrl = "http://localhost:4566/minishop-imagens/test.png";
            _mockStorageService.Setup(service => service.UploadFile(It.IsAny<IFormFile>(), productId))
                .ReturnsAsync(imageUrl);//que retorna a url (key) da imagem

            //Crio uma instancia de ProductImageResponse
            var productImageResponse = new ProductImageResponse
            {
                Url = imageUrl,
                ProductId = productId,
                Id = 123, //ID da imagem
                Sequencia = 1 //Sequencia da imagem
            };

            //Simula o serviço de cadastro de imagem do produto
            var expectedResponse = new ServiceResponse<ProductImageResponse>(productImageResponse);
            _mockService.Setup(service => service.CadastrarImagem(It.IsAny<IFormFile>(), productId))
                .ReturnsAsync(expectedResponse);

            //Act
            var result = await _controller.Post(productId, files);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as List<ProductImageResponse>;
            Assert.NotNull(response);
            Assert.Single(response); //Verifica se a resposta contém uma única imagem
            Assert.Equal(productImageResponse.Url, response[0].Url);
            Assert.Equal(productImageResponse.ProductId, response[0].ProductId);
            Assert.Equal(productImageResponse.Id, response[0].Id);
            Assert.Equal(productImageResponse.Sequencia, response[0].Sequencia);
        }

        [Fact]
        public async Task Post_ImagemDoProduto_DeveRetornarBadRequestQuandoServicoRetornarErro()
        {
            //Arrange
            int productId = 1;
            var formFile = new FormFile(new MemoryStream(new byte[0]), 0, 0, "testFile", "test.pdf");//file inválido (PDF)
            var files = new List<IFormFile> { formFile };

            //Act
            var result = await _controller.Post(productId, files);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal("Formato de arquivo não suportado. Apenas arquivos PNG, JPG e JPEG são permitidos.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteImage_ImagemDoProduto_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            // Arrange
            int productId = 1;
            var model = new ProductImageDeleteRequest
            {
                ImageIdsToRemove = new List<int> { 123 } //ID da imagem a ser removida
            };

            // Simula o serviço de remoção de imagens
            var expectedResponse = new ServiceResponse<List<ProductImageResponse>>(new List<ProductImageResponse>());
            _mockService.Setup(service => service.RemoverImagem(productId, model))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Delete(productId, model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult.Value);

            // Verifica se o valor retornado é um ServiceResponse<List<ProductImageResponse>> e se a lista de conteúdo está vazia
            Assert.IsType<ServiceResponse<List<ProductImageResponse>>>(okResult.Value);
            var response = okResult.Value as ServiceResponse<List<ProductImageResponse>>;
            Assert.NotNull(response);
            Assert.Empty(response.Conteudo); // Verifica se a lista de conteúdo está vazia
        }

        [Fact]
        public async Task DeleteImage_ImagemDoProduto_DeveRetornarBadRequestQuandoServicoRetornarErro()
        {
            //Arrange
            int productId = 1;

            // Simula o serviço de remoção de imagem do produto
            var expectedResponse = new ServiceResponse<List<ProductImageResponse>>("Erro ao remover imagem com ID 123.");
            _mockService.Setup(service => service.RemoverImagem(productId, It.IsAny<ProductImageDeleteRequest>()))
                .ReturnsAsync(expectedResponse);

            //Act
            var result = await _controller.Delete(productId, new ProductImageDeleteRequest());

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<ServiceResponse<List<ProductImageResponse>>>(badRequestResult.Value);
            var response = badRequestResult.Value as ServiceResponse<List<ProductImageResponse>>;
            Assert.False(response.Sucesso);
            Assert.Equal("Erro ao remover imagem com ID 123.", response.Mensagem);
        }

        [Fact]
        public async Task Put_ImagemDoProduto_AlterarOrdem_DeveRetornarOkComImagensAtualizadas()
        {
            // Arrange
            int productId = 1;
            var model = new ProductImageOrderUpdateRequest
            {
                ImageIds = new List<int> { 3, 1, 2 }//IDs de imagens na nova ordem
            };

            var existingProduct = new Product { Id = productId };
            var existingImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, ProductId = productId, Sequencia = 1 },
                new ProductImage { Id = 2, ProductId = productId, Sequencia = 2 },
                new ProductImage { Id = 3, ProductId = productId, Sequencia = 3 }
            };
            var updatedImages = new List<ProductImage>
            {
                new ProductImage { Id = 3, ProductId = productId, Sequencia = 1 },
                new ProductImage { Id = 1, ProductId = productId, Sequencia = 2 },
                new ProductImage { Id = 2, ProductId = productId, Sequencia = 3 }
            };

            //Configura o mock do serviço para retornar as imagens atualizadas
            var expectedResponse = new ServiceResponse<List<ProductImageResponse>>(
                updatedImages.Select(image => new ProductImageResponse(image)).ToList());
            _mockService.Setup(service => service.AlterarOrdemImagens(productId, model))
                        .ReturnsAsync(expectedResponse);

            //Act
            var result = await _controller.Put(productId, model);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ServiceResponse<List<ProductImageResponse>>;
            Assert.NotNull(response);
            Assert.Equal(updatedImages.Count, response.Conteudo.Count);//Verifica se o response contém a mesma quantidade de imagens
            for (int i = 0; i < updatedImages.Count; i++)
            {
                Assert.Equal(updatedImages[i].Id, response.Conteudo[i].Id);// Verifica se os IDs das imagens estão corretos
                Assert.Equal(i + 1, response.Conteudo[i].Sequencia);//Verifica se a sequência foi atualizada corretamente
            }
        }

        [Fact]
        public async Task Put_ImagemDoProduto_AlterarOrdem_DeveRetornarBadRequestComIDsInvalidos()
        {
            //Arrange
            int productId = 1;
            var model = new ProductImageOrderUpdateRequest
            {
                ImageIds = new List<int> { 3, 1, 99 } //IDs de imagens com um ID inválido (99)
            };

            //Simula o serviço de edição de imagens de produtos para retornar um erro com IDs inválidos
            var errorMessage = "A lista de IDs de imagens contém IDs inválidos ou não pertencentes ao produto.";
            var errorResponse = new ServiceResponse<List<ProductImageResponse>>(errorMessage);
            _mockService.Setup(service => service.AlterarOrdemImagens(productId, model))
                        .ReturnsAsync(errorResponse);

            //Act
            var result = await _controller.Put(productId, model);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsType<ServiceResponse<List<ProductImageResponse>>>(badRequestResult.Value);
            var response = badRequestResult.Value as ServiceResponse<List<ProductImageResponse>>;
            Assert.Equal(errorMessage, response.Mensagem);
        }

    }
}



