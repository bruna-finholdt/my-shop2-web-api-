using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Services;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class ProductsService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly ProductsRepository _productsRepository;
        private readonly SuppliersRepository _suppliersRepository;

        public ProductsService(ProductsRepository productsRepository, SuppliersRepository suppliersRepository)
        {
            _productsRepository = productsRepository;
            _suppliersRepository = suppliersRepository;
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

            if (id.GetType() != typeof(int))
            {
                return new ServiceResponse<ProductsCompletoResponse>(
                    "O valor de Id não condiz com formato esperado"
                );
            }

            var supplier = await _suppliersRepository.PesquisaPorId(product.SupplierId);

            return new ServiceResponse<ProductsCompletoResponse>(
                new ProductsCompletoResponse(product, supplier)
            );
            //pra pesquisa de customer por id, usa-se o CustomerCompletoResponse (com tds as informações)
        }
    }
}
