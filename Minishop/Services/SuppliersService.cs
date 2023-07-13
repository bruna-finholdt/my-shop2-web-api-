using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class SuppliersService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly SuppliersRepository _suppliersRepository;
        private readonly ProductsRepository _productsRepository;

        public SuppliersService(SuppliersRepository suppliersRepository, ProductsRepository productsRepository)
        {
            _suppliersRepository = suppliersRepository;
            _productsRepository = productsRepository;
        }

        public async Task<int> Contar()
        {
            int quantidade = await _suppliersRepository.Contagem();

            return quantidade;
        }

        public async Task<ServicePagedResponse<SuppliersResponse>> Pesquisar(PageQueryRequest queryResquest)
        {
            //Lista Customers com paginação
            {
                // Consulta itens no banco
                var listaPesquisa = await _suppliersRepository.Pesquisar(
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
                // Conta itens do banco
                var contagem = await _suppliersRepository.Contagem();
                // Transforma Product em ProductResponse
                var listaConvertida = listaPesquisa
                    .Select(supplier => new SuppliersResponse(supplier));

                // Cria resultado com paginação
                return new ServicePagedResponse<SuppliersResponse>(
                    listaConvertida,
                    contagem,
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
            }
            //No método de listagem de todos os suppliers, os usos do método Select da biblioteca Linq
            //funcionam como um transformador para cada objeto da lista;

        }
        public async Task<ServiceResponse<SuppliersCompletoResponse>> PesquisaPorId(int id)
        //Para usar um método async, devemos colocar async na assinatura, Task<> no retorno e colocar o
        //await na chamada de qualquer método async interno.
        {
            var supplier = await _suppliersRepository.PesquisaPorId(id);
            if (supplier == null)
            {
                return new ServiceResponse<SuppliersCompletoResponse>(
                    "Fornecedor não encontrado"
                );
            }

            //if (id.GetType() != typeof(int))
            //{
            //    return new ServiceResponse<ProductsCompletoResponse>(
            //        "O valor de Id não condiz com formato esperado"
            //    );
            //}

            var products = await _productsRepository.PesquisarSupplierId(id);

            return new ServiceResponse<SuppliersCompletoResponse>(
                new SuppliersCompletoResponse(supplier, products)
            );
            //pra pesquisa de customer por id, usa-se o CustomerCompletoResponse (com tds as informações)
        }

    }
}
