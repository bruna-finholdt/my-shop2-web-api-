using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Services.Base;

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
    }
}
