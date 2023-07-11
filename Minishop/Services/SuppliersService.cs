using Minishop.DAL.Repositories;

namespace Minishop.Services
{
    public class SuppliersService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly SuppliersRepository _suppliersRepository;

        public SuppliersService(SuppliersRepository suppliersRepository)
        {
            _suppliersRepository = suppliersRepository;
        }

        public async Task<int> Contar()
        {
            int quantidade = await _suppliersRepository.Contagem();

            return quantidade;
        }
    }
}
