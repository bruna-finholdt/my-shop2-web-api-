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
        private readonly Minishop2023Context _context;

        //usando o CustomersRepository via injeção de dependência:
        private readonly IProductsRepository _productsRepository;


        public ProductsService(Minishop2023Context context, IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
            _context = context;
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

            // Verificar se o ID do fornecedor existe na base de dados
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == novo.SupplierId);
            if (!supplierExists)
            {
                return new ServiceResponse<ProductsResponse>("Fornecedor não encontrado.");
            }

            var produto = new Product()
            {
                ProductName = novo.ProductName,
                SupplierId = novo.SupplierId,
                UnitPrice = novo.UnitPrice,
                IsDiscontinued = novo.IsDiscontinued,
                PackageName = novo.PackageName
            };

            await _productsRepository.Cadastrar(produto);

            return new ServiceResponse<ProductsResponse>(new ProductsResponse(produto));
        }


        public async Task<ServiceResponse<Product>> Editar(int id, ProductUpdateRequest model)
        {
            var resultado = _context.Products.FirstOrDefault(x => x.Id == id);

            if (resultado == null)
            {
                return new ServiceResponse<Product>("Produto não encontrado");
            }
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == model.SupplierId);
            if (!supplierExists)
            {
                return new ServiceResponse<Product>("Fornecedor não encontrado.");
            }

            resultado.ProductName = model.ProductName;
            resultado.UnitPrice = model.UnitPrice;
            resultado.IsDiscontinued = model.IsDiscontinued;
            resultado.PackageName = model.PackageName;
            resultado.SupplierId = model.SupplierId;

            _context.Entry(resultado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ServiceResponse<Product>(resultado);
        }



    }
}
