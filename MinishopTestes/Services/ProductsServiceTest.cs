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
using Microsoft.AspNetCore.Http;

namespace Minishop.Tests.Services
{
    public class ProductsServiceTest
    {
        private readonly Mock<IProductsRepository> _mockRepository;
        private readonly ProductsService _productsService;
        private readonly Mock<IStorageService> _mockStorageService; // Assuming you also have a mock for IStorageService
        private readonly Mock<ISuppliersRepository> _mockSuppliersRepository;

        public ProductsServiceTest()
        {
            _mockRepository = new Mock<IProductsRepository>();
            _mockSuppliersRepository = new Mock<ISuppliersRepository>();
            _mockStorageService = new Mock<IStorageService>(); // Initialize the mock for IStorageService
            _productsService = new ProductsService(_mockRepository.Object, _mockSuppliersRepository.Object, _mockStorageService.Object);
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

        [Fact]
        public async Task Pesquisar_DeveRetornarListagem()
        {
            // Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 2
            };

            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Prod1", SupplierId = 1, UnitPrice = 100, IsDiscontinued = false, PackageName = "1 pacote" },
                new Product { Id = 2, ProductName = "Prod2", SupplierId = 2, UnitPrice = 50, IsDiscontinued = false, PackageName = "1 pacote" },

            };

            _mockRepository.Setup(repo => repo.Pesquisar(1, 2)).ReturnsAsync(products);

            // Act
            var result = await _productsService.Pesquisar(queryRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Conteudo.Count()); // Verifica se a quantidade de produtos retornados é a esperada.

            // Verifica se os dados dos produtos estão corretos
            var productsResponseList = result.Conteudo.ToList();
            Assert.Equal(products[0].Id, productsResponseList[0].Id);
            Assert.Equal(products[0].ProductName, productsResponseList[0].ProductName);
            Assert.Equal(products[0].SupplierId, productsResponseList[0].SupplierId);
            Assert.Equal(products[0].UnitPrice, productsResponseList[0].UnitPrice);
            Assert.Equal(products[0].IsDiscontinued, productsResponseList[0].IsDiscontinued);
            Assert.Equal(products[0].PackageName, productsResponseList[0].PackageName);

            Assert.Equal(products[1].Id, productsResponseList[1].Id);
            Assert.Equal(products[1].ProductName, productsResponseList[1].ProductName);
            Assert.Equal(products[1].SupplierId, productsResponseList[1].SupplierId);
            Assert.Equal(products[1].UnitPrice, productsResponseList[1].UnitPrice);
            Assert.Equal(products[1].IsDiscontinued, productsResponseList[1].IsDiscontinued);
            Assert.Equal(products[1].PackageName, productsResponseList[1].PackageName);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarProdutoQuandoIdExistente()
        {
            // Arrange
            int id = 1;
            var existingProduct = new Product
            {
                Id = id,
                ProductName = "Produto A",
                SupplierId = 1,
                UnitPrice = 10.0m,
                IsDiscontinued = false,
                PackageName = "Pacote 1"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingProduct);

            // Act
            var result = await _productsService.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingProduct.Id, result.Conteudo.Id);
            Assert.Equal(existingProduct.ProductName, result.Conteudo.ProductName);
            Assert.Equal(existingProduct.SupplierId, result.Conteudo.SupplierId);
            Assert.Equal(existingProduct.UnitPrice, result.Conteudo.UnitPrice);
            Assert.Equal(existingProduct.IsDiscontinued, result.Conteudo.IsDiscontinued);
            Assert.Equal(existingProduct.PackageName, result.Conteudo.PackageName);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarNuloQuandoIdNaoExistente()
        {
            // Arrange
            int idNaoExistente = 999; // ID que não existe no repositório
            _mockRepository.Setup(repo => repo.PesquisaPorId(idNaoExistente)).ReturnsAsync((Product)null);

            // Act
            var result = await _productsService.PesquisaPorId(idNaoExistente);

            // Assert
            Assert.Null(result.Conteudo);
            Assert.False(result.Sucesso);
            Assert.Equal("Produto não encontrado", result.Mensagem);
        }

        [Fact]
        public async Task Cadastrar_DeveSalvarProdutoQuandoRequestValido()
        {
            // Arrange
            var request = new ProductCreateRequest
            {
                ProductName = "Product A",
                SupplierId = 1,
                UnitPrice = 50,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            // Set up the behavior of ISuppliersRepository for the scenario where the supplier exists (returning true)
            _mockSuppliersRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync(new Supplier());


            Product savedProduct = null;
            _mockRepository.Setup(repo => repo.Cadastrar(It.IsAny<Product>()))
                .Callback<Product>(product => savedProduct = product)
                .ReturnsAsync((Product product) => product);

            // Act
            var response = await _productsService.Cadastrar(request);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(request.ProductName, savedProduct.ProductName);
            Assert.Equal(request.SupplierId, savedProduct.SupplierId);
            Assert.Equal(request.UnitPrice, savedProduct.UnitPrice);
            Assert.Equal(request.IsDiscontinued, savedProduct.IsDiscontinued);
            Assert.Equal(request.PackageName, savedProduct.PackageName);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoFornecedorNaoEncontrado()
        {
            // Arrange
            var request = new ProductCreateRequest
            {
                ProductName = "Product A",
                SupplierId = 99, // Um ID de fornecedor que não existe no banco de dados
                UnitPrice = 50,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            // Set up the behavior of ISuppliersRepository for the scenario where the supplier does not exist (returning null)
            _mockSuppliersRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync((Supplier)null);

            // Act
            var response = await _productsService.Cadastrar(request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("Fornecedor não encontrado.", response.Mensagem);
        }

        [Fact]
        public async Task Editar_DeveSalvarProdutoQuandoRequestValido()
        {
            // Arrange
            int id = 1;
            var request = new ProductUpdateRequest
            {
                ProductName = "Updated Product A",
                SupplierId = 2, // Novo ID de fornecedor existente no banco de dados
                UnitPrice = 60,
                IsDiscontinued = true,
                PackageName = "Updated Package 1"
            };

            var existingProduct = new Product
            {
                Id = id,
                ProductName = "Product A",
                SupplierId = 1,
                UnitPrice = 50,
                IsDiscontinued = false,
                PackageName = "Package 1"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingProduct);
            _mockSuppliersRepository.Setup(repo => repo.PesquisaPorId(request.SupplierId.Value)).ReturnsAsync(new Supplier()); // Use PesquisaPorId to check supplier existence

            Product savedProduct = null;
            _mockRepository.Setup(repo => repo.Editar(It.IsAny<Product>()))
                .Callback<Product>(product => savedProduct = product)
                .ReturnsAsync((Product product) => product);

            // Act
            var response = await _productsService.Editar(id, request);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(request.ProductName, savedProduct.ProductName);
            Assert.Equal(request.SupplierId, savedProduct.SupplierId);
            Assert.Equal(request.UnitPrice, savedProduct.UnitPrice);
            Assert.Equal(request.IsDiscontinued, savedProduct.IsDiscontinued);
            Assert.Equal(request.PackageName, savedProduct.PackageName);
        }

        [Fact]
        public async Task Editar_DeveRetornarErroQuandoProdutoNaoEncontrado()
        {
            // Arrange
            int id = 99; // ID de produto que não existe no banco de dados
            var request = new ProductUpdateRequest
            {
                ProductName = "Updated Product A",
                SupplierId = 2,
                UnitPrice = 60,
                IsDiscontinued = true,
                PackageName = "Updated Package 1"
            };

            Product existingProduct = null;
            _mockRepository.Setup(repo => repo.PesquisaPorId(id)).ReturnsAsync(existingProduct);

            // Act
            var response = await _productsService.Editar(id, request);

            // Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("Produto não encontrado.", response.Mensagem);
        }

        [Fact]
        public async Task CadastrarImagem_DeveSalvarImagemQuandoProdutoExistente()
        {
            //Arrange
            var productId = 1;
            var fileMock = new Mock<IFormFile>();

            _mockRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync(new Product { Id = productId });

            _mockRepository.Setup(repo => repo.GetHighestSequence(productId)).ReturnsAsync(2);

            var imageUrl = "https://example.com/image.jpg";
            _mockStorageService.Setup(service => service.UploadFile(It.IsAny<IFormFile>(), productId)).ReturnsAsync(imageUrl);

            ProductImage savedProductImage = null;
            _mockRepository.Setup(repo => repo.CadastrarImagem(It.IsAny<ProductImage>()))
                .Callback<ProductImage>(image => savedProductImage = image)
                .ReturnsAsync((ProductImage image) => image);

            //Act
            var response = await _productsService.CadastrarImagem(fileMock.Object, productId);

            //Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(imageUrl, response.Conteudo.Url);
            Assert.Equal(productId, savedProductImage.ProductId);
            Assert.Equal(imageUrl, savedProductImage.Url);
            Assert.Equal(3, savedProductImage.Sequencia);//Expected: A próxima sequencia após o 2
        }

        [Fact]
        public async Task CadastrarImagem_DeveRetornarErroQuandoProdutoNaoExiste()
        {
            //Arrange
            var productId = 1;
            var fileMock = new Mock<IFormFile>();

            //Simula o repository para retornar null, indicando que o produto não existe
            _mockRepository.Setup(repo => repo.PesquisaPorId(It.IsAny<int>())).ReturnsAsync((Product)null);

            //Act
            var response = await _productsService.CadastrarImagem(fileMock.Object, productId);

            //Assert
            Assert.False(response.Sucesso);//Retorna false indicando erro
            Assert.Null(response.Conteudo);//Conteúdo null devido ao erro
            Assert.Equal("Produto não encontrado.", response.Mensagem);//Verifica a mensagem de erro
        }


        [Fact]
        public async Task EditarImagem_DeveEditarImagensComSucesso()
        {
            //Arrange
            var productId = 1;

            //Mock imagens já existentes do produto com id 1
            var existingImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 2, Url = "url2", Sequencia = 2, ProductId = productId }
            };

            //Lista de IFormFile com imagens simuladas para serem adicionadas na edição do prod de id 1
            var mockImages = new List<IFormFile>
            {
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("Imagem1")), 0, 0, "imagem1.jpg", "imagem1.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("Imagem2")), 0, 0, "imagem2.jpg", "imagem2.jpg")
            };

            //Mock do ProductUpdateRequest
            var model = new ProductImageUpdateRequest
            {
                NewImages = mockImages,//add 2 imagens
                ImageIdsToRemove = new List<int> { 1, 2 }//remove as imagens existentes de id 1 e 2
            };

            //Lista com as novas imagens que foram adicionadas na edição do prod de id 1
            var updatedImages = new List<ProductImage>
            {
                new ProductImage { Id = 3, Url = "url3", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 4, Url = "url4", Sequencia = 2, ProductId = productId }
            };

            //Simula o serviço de armazenamento
            _mockStorageService.Setup(service => service.UploadFile(It.IsAny<IFormFile>(), productId))
                .ReturnsAsync("url3");
            _mockStorageService.Setup(service => service.UploadFile(It.IsAny<IFormFile>(), productId))
                .ReturnsAsync("url4");

            //Simula o repository
            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });

            _mockRepository.Setup(repo => repo.RemoverImagem(It.IsAny<int>())).ReturnsAsync(true);

            //GetImagesByProductId (antes de add as novas imagens/remover as imagens existentes)
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId))
                .ReturnsAsync(existingImages);// returna a lista de imagens original

            List<ProductImage> addedImages = new List<ProductImage>();

            _mockRepository.Setup(repo => repo.CadastrarImagem(It.IsAny<ProductImage>()))
                .ReturnsAsync((ProductImage image) =>
                {
                    addedImages.Add(image);
                    return image;
                });

            _mockRepository.Setup(repo => repo.ReorganizarSequenciaDeImagens(productId)).ReturnsAsync(true);

            //GetImagesByProductId (dps de add as novas imagens/remover as imagens existentes)
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId))
                .ReturnsAsync(updatedImages);//retorna a nova lista de imagens (atualizada)

            // Act
            var response = await _productsService.EditarImagem(productId, model);

            // Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(2, response.Conteudo.Count);
            Assert.Equal("url3", response.Conteudo[0].Url);
            Assert.Equal("url4", response.Conteudo[1].Url);
            Assert.Equal(1, updatedImages[0].Sequencia);
            Assert.Equal(2, updatedImages[1].Sequencia);
        }

        [Fact]
        public async Task EditarImagem_DeveRetornarErroQuandoTiposDeArquivoInadequados()
        {
            //Arrange
            var productId = 1;

            //Mock imagens existentes
            var existingImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 2, Url = "url2", Sequencia = 2, ProductId = productId }
            };

            //Cria uma lista de IFormFile com tipos de arquivo inválidos
            var invalidImages = new List<IFormFile>
            {
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("Arquivo de texto")), 0, 0, "arquivo.txt", "arquivo.txt"),

            };

            var model = new ProductImageUpdateRequest
            {
                NewImages = invalidImages,
                ImageIdsToRemove = new List<int> { 1, 2 }
            };

            //Simula o repository para retornar um produto existente
            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });

            //Simula o repository para retornar imagens existentes
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId)).ReturnsAsync(existingImages);

            //Simula o serviço de armazenamento para retornar um erro quando tipos de arquivo inválidos são enviados
            _mockStorageService.Setup(service => service.UploadFile(It.IsAny<IFormFile>(), productId))
                .ThrowsAsync(new ArgumentException("Formato de arquivo não suportado. Apenas arquivos PNG, JPG e JPEG são permitidos."));

            //Act
            var response = await _productsService.EditarImagem(productId, model);

            //Assert
            Assert.False(response.Sucesso); // Deve retornar false indicando erro
            Assert.Null(response.Conteudo); // Conteúdo deve ser null em caso de erro
            Assert.Equal("Formato de arquivo não suportado. Apenas arquivos PNG, JPG e JPEG são permitidos.", response.Mensagem); // Verifica a mensagem de erro
        }

        [Fact]
        public async Task AlterarOrdemImagens_DeveAtualizarSequenciaDeImagensComSucesso()
        {
            //Arrange
            var productId = 1;
            var model = new ProductImageOrderUpdateRequest
            {
                ImageIds = new List<int> { 2, 1 } //A ordem das imagens será invertida
            };
            var images = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 2, Url = "url2", Sequencia = 2, ProductId = productId }
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId)).ReturnsAsync(images);

            //Atualização da sequência das imagens 
            _mockRepository.Setup(repo => repo.ReorganizarSequenciaDeImagens(productId)).ReturnsAsync(true);

            //Act
            var response = await _productsService.AlterarOrdemImagens(productId, model);

            //Assert
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(2, response.Conteudo.Count);
            Assert.Equal(2, response.Conteudo[0].Sequencia);
            Assert.Equal(1, response.Conteudo[1].Sequencia);
        }

        [Fact]
        public async Task AlterarOrdemImagens_DeveRetornarErroQuandoListaDeIdsContemIdsInvalidos()
        {
            //Arrange
            var productId = 1;
            var model = new ProductImageOrderUpdateRequest
            {
                ImageIds = new List<int> { 1, 2 }
            };
            var images = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1", Sequencia = 1, ProductId = productId },
                new ProductImage { Id = 3, Url = "url3", Sequencia = 2, ProductId = productId }
                // Aqui temos as imagens com IDs 1 e 3, mas estamos fornecendo IDs 1 e 2
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(productId)).ReturnsAsync(new Product { Id = productId });
            _mockRepository.Setup(repo => repo.GetImagesByProductId(productId)).ReturnsAsync(images);

            //Act
            var response = await _productsService.AlterarOrdemImagens(productId, model);

            //Assert
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal("A lista de IDs de imagens contém IDs inválidos ou não pertencentes ao produto.", response.Mensagem);
        }
    }
}
