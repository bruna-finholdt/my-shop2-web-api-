using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class OrdersService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly OrdersRepository _ordersRepository;

        public OrdersService(OrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<int> Contar()
        {
            int quantidade = await _ordersRepository.ContagemPedidos();

            return quantidade;
        }

        public async Task<ServicePagedResponse<CustomerOrderResponse>> Pesquisar(PageQueryRequest queryResquest)
        {
            //Lista Customers com paginação
            {
                // Consulta itens no banco
                var listaPesquisa = await _ordersRepository.Pesquisar(
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
                // Conta itens do banco
                var contagem = await _ordersRepository.Contagem();
                // Transforma Product em ProductResponse
                var listaConvertida = listaPesquisa
                    .Select(order => new CustomerOrderResponse(order));

                // Cria resultado com paginação
                return new ServicePagedResponse<CustomerOrderResponse>(
                    listaConvertida,
                    contagem,
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
            }
            //No método de listagem de todas as orders, os usos do método Select da biblioteca Linq
            //funcionam como um transformador para cada objeto da lista;

        }
    }
}
