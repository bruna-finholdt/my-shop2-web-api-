using Castle.Core.Resource;
using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Minishop.Tests.Repositories
{
    public class ProductsRepositoryTest : IDisposable
    {
        private readonly Minishop2023Context _dbContext;
        private readonly ProductsRepository _productsRepository;
        private readonly Mock<IStorageService> _mockStorageService;

        public ProductsRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                    .UseInMemoryDatabase(databaseName: "TestProductsDatabase")
                    .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
            _dbContext.Database.EnsureDeleted(); // Clear the database before each test
            _dbContext.Suppliers.Add(new Supplier { Id = 1, CompanyName = "Supplier A", City = "City A", Uf = "MG", Cnpj = "123456789", Email = "supplierA@example.com" });
            _dbContext.SaveChanges();
            _mockStorageService = new Mock<IStorageService>();
            _productsRepository = new ProductsRepository(_dbContext, _mockStorageService.Object);
        }

        [Fact]
        public async Task Contagem_DeveRetornarQuantidadeCorretaDeRegistros()
        {
            // Arrange: Limpe o contexto antes de adicionar os produtos
            _dbContext.Products.RemoveRange(_dbContext.Products);

            // Adicionando alguns registros ao banco de dados em memória.
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

            // Adicionando produtos ativos e inativos
            var activeProducts = new List<Product>
            {
                new Product { Id = 3, ProductName = "Prod3", SupplierId = 3, UnitPrice = 70, IsDiscontinued = false, PackageName = "1 pacote" },
                new Product { Id = 4, ProductName = "Prod4", SupplierId = 4, UnitPrice = 60, IsDiscontinued = false, PackageName = "1 pacote" },
            };

            var inactiveProducts = new List<Product>
            {
                new Product { Id = 5, ProductName = "Prod5", SupplierId = 5, UnitPrice = 80, IsDiscontinued = true, PackageName = "1 pacote" },
            };

            // Obtém a contagem atual de produtos antes da inserção dos produtos ativos e inativos
            var currentCount = await _productsRepository.Contagem();

            _dbContext.Products.AddRange(activeProducts);
            _dbContext.Products.AddRange(inactiveProducts);
            _dbContext.SaveChanges();

            // Obtém a contagem de produtos ativos e inativos após a inserção
            var activeCount = await _productsRepository.ContagemProdutosAtivos();
            var inactiveCount = await _productsRepository.ContagemProdutosInativos();

            // Assert
            Assert.Equal(products.Count, currentCount); // Verifica se a contagem inicial ainda é igual ao número esperado de registros.
            Assert.Equal(products.Count + activeProducts.Count, activeCount); // Verifica se a contagem de produtos ativos é igual ao número esperado de registros ativos.
            Assert.Equal(inactiveProducts.Count, inactiveCount); // Verifica se a contagem de produtos inativos é igual ao número esperado de registros inativos.
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarListagem()
        {
            // Arrange: Adicione alguns registros ao banco de dados em memória.
            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Produto A", SupplierId = 1, UnitPrice = 10.0m, IsDiscontinued = false, PackageName = "Pacote 1" },
                new Product { Id = 2, ProductName = "Produto B", SupplierId = 1, UnitPrice = 15.5m, IsDiscontinued = false, PackageName = "Pacote 2" },
                new Product { Id = 3, ProductName = "Produto C", SupplierId = 2, UnitPrice = 20.25m, IsDiscontinued = true, PackageName = "Pacote 3" }
            };

            _dbContext.Products.AddRange(products);
            _dbContext.SaveChanges();

            // Act
            var result = await _productsRepository.Pesquisar(paginaAtual: 1, qtdPagina: 3);

            // Assert
            // Verifique se o resultado corresponde aos produtos adicionados
            Assert.Equal(products.Count, result.Count);
            Assert.Equal(products[0].Id, result[0].Id);
            Assert.Equal(products[0].ProductName, result[0].ProductName);
            Assert.Equal(products[0].SupplierId, result[0].SupplierId);
            Assert.Equal(products[0].UnitPrice, result[0].UnitPrice);
            Assert.Equal(products[0].IsDiscontinued, result[0].IsDiscontinued);
            Assert.Equal(products[0].PackageName, result[0].PackageName);
            Assert.Equal(products[1].Id, result[1].Id);
            Assert.Equal(products[1].ProductName, result[1].ProductName);
            Assert.Equal(products[1].SupplierId, result[1].SupplierId);
            Assert.Equal(products[1].UnitPrice, result[1].UnitPrice);
            Assert.Equal(products[1].IsDiscontinued, result[1].IsDiscontinued);
            Assert.Equal(products[1].PackageName, result[1].PackageName);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarProdutoQuandoIdExistente()
        {
            // Arrange
            int id = 7;

            var existingProduct = new Product
            {
                Id = 7,
                ProductName = "Produto F",
                SupplierId = 1,
                UnitPrice = 100,
                IsDiscontinued = false,
                PackageName = "Pacote 1"
            };

            _dbContext.Products.Add(existingProduct);
            _dbContext.SaveChanges();

            // Act
            var result = await _productsRepository.PesquisaPorId(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingProduct.Id, result.Id);
            Assert.Equal(existingProduct.ProductName, result.ProductName);
            Assert.Equal(existingProduct.SupplierId, result.SupplierId);
            Assert.Equal(existingProduct.UnitPrice, result.UnitPrice);
            Assert.Equal(existingProduct.IsDiscontinued, result.IsDiscontinued);
            Assert.Equal(existingProduct.PackageName, result.PackageName);
        }

        [Fact]
        public async Task Cadastrar_DeveCriarNovoProduct()
        {
            //Arrange
            var newProduct = new Product
            {
                ProductName = "New Product",
                SupplierId = 1,
                UnitPrice = 50,
                IsDiscontinued = false,
                PackageName = "1 pacote"
            };

            //Act
            var createdProduct = await _productsRepository.Cadastrar(newProduct);

            //Assert
            Assert.NotNull(createdProduct);
            Assert.Contains(newProduct, _dbContext.Products);//verifica se o new product está no db
        }

        [Fact]
        public async Task Editar_DeveAtualizarProductExistente()
        {
            //Arrange
            var existingProduct = new Product
            {
                Id = 1,
                ProductName = "Prod1",
                SupplierId = 1,
                UnitPrice = 100,
                IsDiscontinued = false,
                PackageName = "1 pacote"
            };

            _dbContext.Products.Add(existingProduct);
            _dbContext.SaveChanges();

            //Modifica os dados do Product existente
            existingProduct.ProductName = "Updated Product";
            existingProduct.UnitPrice = 200;

            //Act
            var updatedProduct = await _productsRepository.Editar(existingProduct);

            //Assert
            Assert.NotNull(updatedProduct);
            Assert.Equal(existingProduct.Id, updatedProduct.Id);
            Assert.Equal("Updated Product", updatedProduct.ProductName);
            Assert.Equal(200, updatedProduct.UnitPrice);
            var productInContext = _dbContext.Products.Find(existingProduct.Id);
            Assert.NotNull(productInContext);
            Assert.Equal("Updated Product", productInContext.ProductName);
            Assert.Equal(200, productInContext.UnitPrice);
        }

        [Fact]
        public async Task CadastrarImagem_DeveCadastrarImagemComSucesso()
        {
            //Arrange
            var productId = 1;
            var existingProduct = new Product
            {
                Id = 1,
                ProductName = "Prod1",
                SupplierId = 1,
                UnitPrice = 100,
                IsDiscontinued = false,
                PackageName = "1 pacote"
            };
            _dbContext.Products.Add(existingProduct);
            _dbContext.SaveChanges();

            var productImage = new ProductImage { Id = 1, ProductId = productId, Url = "urlNewImageProduct.jpg", Sequencia = 1 };

            //Act
            var result = await _productsRepository.CadastrarImagem(productImage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("urlNewImageProduct.jpg", result.Url);
            Assert.Contains(productImage, _dbContext.ProductImages);//verifica se a productImage está no db
        }

        [Fact]
        public async Task RemoverImagem_DeveRemoverImagemComSucesso()
        {
            // Arrange
            var imageId = 1;
            var productImage = new ProductImage { Id = imageId, ProductId = 1, Url = "urlNewImageProduct.jpg", Sequencia = 1 };
            _dbContext.ProductImages.Add(productImage);
            _dbContext.SaveChanges();

            _mockStorageService.Setup(repo => repo.RemoveImageFromBucket(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _productsRepository.RemoverImagem(imageId);

            // Assert
            Assert.True(result);
            Assert.Null(_dbContext.ProductImages.Find(imageId));//Verifica se a imagem foi removida do db
        }

        [Fact]
        public async Task GetImageCount_DeveRetornarQuantidadeCorretaDeImagens()
        {
            //Arrange
            var productId = 1;
            var productImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, ProductId = productId, Url = "url1.jpg", Sequencia = 1 },
                new ProductImage { Id = 2, ProductId = productId, Url = "url2.jpg", Sequencia = 2 },
                new ProductImage { Id = 3, ProductId = productId, Url = "url3.jpg", Sequencia = 3 }
            };

            _dbContext.ProductImages.AddRange(productImages);
            _dbContext.SaveChanges();

            //Act
            var result = await _productsRepository.GetImageCount(productId);

            //Assert
            Assert.Equal(productImages.Count, result);
        }

        [Fact]
        public async Task ReorganizarSequenciaDeImagens_DeveRetornarTrueQuandoReorganizar()
        {
            // Arrange
            var productId = 1;
            var productImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, ProductId = productId, Url = "url1.jpg", Sequencia = 3 },
                new ProductImage { Id = 2, ProductId = productId, Url = "url2.jpg", Sequencia = 1 },
                new ProductImage { Id = 3, ProductId = productId, Url = "url3.jpg", Sequencia = 2 }
            };

            _dbContext.ProductImages.AddRange(productImages);
            _dbContext.SaveChanges();

            //Act
            var result = await _productsRepository.ReorganizarSequenciaDeImagens(productId);

            //Assert
            Assert.True(result);

            //Verifique se a sequência foi reorganizada corretamente no db (após imagem ser deletada)
            var updatedImages = await _dbContext.ProductImages
                .Where(image => image.ProductId == productId)
                .OrderBy(image => image.Sequencia)
                .ToListAsync();

            Assert.Equal(1, updatedImages[0].Sequencia);
            Assert.Equal(2, updatedImages[1].Sequencia);
            Assert.Equal(3, updatedImages[2].Sequencia);
        }

        [Fact]
        public async Task GetImageById_DeveRetornarImagemCorretaQuandoPassadoIdValido()
        {
            // Arrange
            var imageId = 1;
            var productImage = new ProductImage { Id = imageId, ProductId = 1, Url = "url.jpg", Sequencia = 1 };
            _dbContext.ProductImages.Add(productImage);
            _dbContext.SaveChanges();

            // Act
            var result = await _productsRepository.GetImageById(imageId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(imageId, result.Id);
            Assert.Equal(1, result.ProductId);
            Assert.Equal("url.jpg", result.Url);
        }

        [Fact]
        public async Task GetHighestSequence_DeveRetornarAMaiorSequenciaDeImagemDeProduto()
        {
            //Arrange
            var productId = 1;
            var productImages = new List<ProductImage>
            {
                new ProductImage { Id = 1, Url = "url1.jpg", ProductId = productId, Sequencia = 2 },
                new ProductImage { Id = 2, Url = "url2.jpg", ProductId = productId, Sequencia = 3 },
                new ProductImage { Id = 3, Url = "url3.jpg", ProductId = productId, Sequencia = 1 }
            };

            _dbContext.ProductImages.AddRange(productImages);
            _dbContext.SaveChanges();

            //Act
            var result = await _productsRepository.GetHighestSequence(productId);

            //Assert
            Assert.Equal(3, result);//O maior valor de sequência é o 3
        }


        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
