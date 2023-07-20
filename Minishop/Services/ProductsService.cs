using Microsoft.EntityFrameworkCore;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class ProductsService : IProductsService
    {
        //usando o Minishop2023Context via injeção de dependência:
        //private readonly ICustomersDbContext _context;

        //usando o CustomersRepository via injeção de dependência:
        private readonly IProductsRepository _productsRepository;


        public ProductsService(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;

        }

        public async Task<ProductCountResponse> Contar()
        {
            int quantidadeTotal = await _productsRepository.Contagem();
            int quantidadeProdutosAtivos = await _productsRepository.ContagemProdutosAtivos();
            int quantidadeProdutosInativos = await _productsRepository.ContagemProdutosInativos();

            var response = new ProductCountResponse
            {
                ItemCount = quantidadeTotal,
                ActiveCount = quantidadeProdutosAtivos,
                InactiveCount = quantidadeProdutosInativos
            };

            return response;
        }

        public async Task<ServicePagedResponse<ProductsResponse>> Pesquisar(PageQueryRequest queryResquest)
        {
            //Lista Customers com paginação
            {
                // Consulta itens no banco
                var listaPesquisa = await _productsRepository.Pesquisar(
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
                // Conta itens do banco
                var contagem = await _productsRepository.Contagem();
                // Transforma Product em ProductResponse
                var listaConvertida = listaPesquisa
                    .Select(product => new ProductsResponse(product));

                // Cria resultado com paginação
                return new ServicePagedResponse<ProductsResponse>(
                    listaConvertida,
                    contagem,
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
            }
            //No método de listagem de todos os products, os usos do método Select da biblioteca Linq
            //funcionam como um transformador para cada objeto da lista;

        }

        public async Task<ServiceResponse<ProductsCompletoResponse>> PesquisaPorId(int id)
        //Para usar um método async, devemos colocar async na assinatura, Task<> no retorno e colocar o
        //await na chamada de qualquer método async interno.
        {
            var product = await _productsRepository.PesquisaPorId(id);


            if (product == null)
            {
                return new ServiceResponse<ProductsCompletoResponse>(
                    "Produto não encontrado"
                );
            }


            return new ServiceResponse<ProductsCompletoResponse>(
                new ProductsCompletoResponse(product)
            );

        }


        public async Task<ServiceResponse<ProductsResponse>> Cadastrar(ProductCreateRequest novo)
        {

            if (novo.UnitPrice == null || !novo.UnitPrice.HasValue || novo.UnitPrice.Value == 0)
            {
                return new ServiceResponse<ProductsResponse>("O preço deve ser informado.");
            }

            // Verificar se o Id de supplier enviado existe na base de dados
            var supplierExists = await _productsRepository.VerificarFornecedorExistente(novo.SupplierId);
            if (!supplierExists)
            {
                return new ServiceResponse<ProductsResponse>("Fornecedor não encontrado.");
            }

            var produto = new Product
            {
                ProductName = novo.ProductName,
                SupplierId = novo.SupplierId,
                UnitPrice = novo.UnitPrice,
                IsDiscontinued = novo.IsDiscontinued,
                PackageName = novo.PackageName
            };

            var createdProduct = await _productsRepository.Cadastrar(produto);

            if (createdProduct == null)
            {
                return new ServiceResponse<ProductsResponse>("Erro ao cadastrar produto.");
            }

            var response = new ProductsResponse(createdProduct);

            return new ServiceResponse<ProductsResponse>(response);
        }


        public async Task<ServiceResponse<ProductsResponse>> Editar(int id, ProductUpdateRequest model)
        {
            // Verificar se o produto com o ID fornecido existe no banco de dados
            var existingProduct = await _productsRepository.PesquisaPorId(id);
            if (existingProduct == null)
            {
                return new ServiceResponse<ProductsResponse>("Produto não encontrado.");
            }

            // Verificar se o ID do fornecedor enviado existe na base de dados (only if SupplierId is provided)
            if (model.SupplierId.HasValue)
            {
                var supplierExists = await _productsRepository.VerificarFornecedorExistente(model.SupplierId.Value);
                if (!supplierExists)
                {
                    return new ServiceResponse<ProductsResponse>("Fornecedor não encontrado.");
                }
            }

            // Atualizar os campos do produto com os valores do modelo, se eles não forem nulos ou vazios
            if (!string.IsNullOrWhiteSpace(model.ProductName))
            {
                existingProduct.ProductName = model.ProductName;
            }

            if (model.SupplierId.HasValue)
            {
                existingProduct.SupplierId = model.SupplierId.Value;
            }

            if (model.UnitPrice.HasValue)
            {
                existingProduct.UnitPrice = model.UnitPrice.Value;
            }

            if (model.IsDiscontinued.HasValue)
            {
                existingProduct.IsDiscontinued = model.IsDiscontinued.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.PackageName))
            {
                existingProduct.PackageName = model.PackageName;
            }

            // Chamar o método Editar do repositório para salvar as alterações no banco de dados
            var updatedProduct = await _productsRepository.Editar(existingProduct);

            // Verificar se a edição foi realizada com sucesso
            if (updatedProduct == null)
            {
                return new ServiceResponse<ProductsResponse>("Erro ao editar produto.");
            }

            // Criar o objeto ProductsResponse para retornar ao cliente
            var response = new ProductsResponse(updatedProduct);

            return new ServiceResponse<ProductsResponse>(response);
        }
    }
}
