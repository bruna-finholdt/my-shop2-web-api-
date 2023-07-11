using Minishop.DAL.Repositories;

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
    }
}
