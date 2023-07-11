using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Services.Base;

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

        public async Task<ServicePagedResponse<CustomerResponse>> Pesquisar(PageQueryRequest queryResquest)
        {
            //Lista Customers com paginação
            {
                // Consulta itens no banco
                var listaPesquisa = await _customersRepository.Pesquisar(
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
                // Conta itens do banco
                var contagem = await _customersRepository.Contagem();
                // Transforma Product em ProductResponse
                var listaConvertida = listaPesquisa
                    .Select(customer => new CustomerResponse(customer));

                // Cria resultado com paginação
                return new ServicePagedResponse<CustomerResponse>(
                    listaConvertida,
                    contagem,
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
            }
            //No método de listagem de todos os customers, os usos do método Select da biblioteca Linq
            //funcionam como um transformador para cada objeto da lista;

        }
    }
}
