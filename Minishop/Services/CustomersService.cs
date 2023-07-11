using Minishop.DAL.Repositories;

namespace Minishop.Services
{
    public class CustomersService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly CustomersRepository _customersRepository;

        public CustomersService(CustomersRepository customersRepository)
        {
            _customersRepository = customersRepository;
        }

        public async Task<int> Contar()
        {
            int quantidade = await _customersRepository.Contagem();

            return quantidade;
        }
    }
}
