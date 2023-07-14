using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class OrdersService : IOrdersService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly OrdersRepository _ordersRepository;
        private readonly OrderItemsRepository _orderItemsRepository;

        public OrdersService(OrdersRepository ordersRepository, OrderItemsRepository orderItemsRepository)
        {
            _ordersRepository = ordersRepository;
            _orderItemsRepository = orderItemsRepository;
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

        public async Task<ServiceResponse<CustomerOrderCompletoResponse>> PesquisaPorId(int id)
        //Para usar um método async, devemos colocar async na assinatura, Task<> no retorno e colocar o
        //await na chamada de qualquer método async interno.
        {
            var customerOrder = await _ordersRepository.PesquisaPorId(id);
            if (customerOrder == null)
            {
                return new ServiceResponse<CustomerOrderCompletoResponse>(
                    "Pedido não encontrado"
                );
            }


            //var orderItems = await _orderItemsRepository.PesquisarCustomerOrderId(id);


            return new ServiceResponse<CustomerOrderCompletoResponse>(
                new CustomerOrderCompletoResponse(customerOrder)
            );

        }
    }
}
