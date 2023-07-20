using Microsoft.EntityFrameworkCore;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class SuppliersService : ISupplierService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly ISuppliersRepository _suppliersRepository;
        //private readonly ICustomersDbContext _context;
        public SuppliersService(ISuppliersRepository suppliersRepository)
        {
            _suppliersRepository = suppliersRepository;
            //_context = context;
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

        {
            var supplier = await _suppliersRepository.PesquisaPorId(id);
            if (supplier == null)
            {
                return new ServiceResponse<SuppliersCompletoResponse>(
                    "Fornecedor não encontrado"
                );
            }
            return new ServiceResponse<SuppliersCompletoResponse>(
                new SuppliersCompletoResponse(supplier)
            );

        }

        public async Task<ServiceResponse<SuppliersResponse>> Cadastrar(SupplierCreateRequest novo)
        {

            // Verificar se o CNPJ já está em uso
            var cnpjExists = await _suppliersRepository.VerificarCnpjExistente(novo.Cnpj);
            if (cnpjExists)
            {
                return new ServiceResponse<SuppliersResponse>("CNPJ duplicado");
            }

            // Verificar se o e-mail já está em uso
            var emailExists = await _suppliersRepository.VerificarEmailExistente(novo.Email);
            if (emailExists)
            {
                return new ServiceResponse<SuppliersResponse>("E-mail duplicado");
            }

            // Convert the 'Uf' value to uppercase before validation and saving
            if (!string.IsNullOrEmpty(novo.Uf))
            {
                novo.Uf = novo.Uf.ToUpper();
            }

            // Verificar se estado corresponde a uma das 27 siglas de estados brasileiros
            var estadosBrasileiros = new[] { "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO" };
            if (!string.IsNullOrEmpty(novo.Uf) && !estadosBrasileiros.Contains(novo.Uf, StringComparer.OrdinalIgnoreCase))
            {
                return new ServiceResponse<SuppliersResponse>("Estado inválido.");
            }

            var supplier = new Supplier()
            {
                CompanyName = novo.CompanyName,
                Cnpj = novo.Cnpj,
                Email = novo.Email,
                Phone = novo.Phone,
                City = novo.City,
                Uf = novo.Uf,
                ContactName = novo.ContactName
            };

            var createdSupplier = await _suppliersRepository.Cadastrar(supplier);

            if (createdSupplier == null)
            {
                return new ServiceResponse<SuppliersResponse>("Erro ao cadastrar fornecedor.");
            }

            var response = new SuppliersResponse(createdSupplier);

            return new ServiceResponse<SuppliersResponse>(response);
        }

        public async Task<ServiceResponse<SuppliersResponse>> Editar(int id, SupplierUpdateRequest model)
        {
            // Verificar se o fornecedor com o ID fornecido existe no banco de dados
            var existingSupplier = await _suppliersRepository.PesquisaPorId(id);
            if (existingSupplier == null)
            {
                return new ServiceResponse<SuppliersResponse>("Fornecedor não encontrado.");
            }

            // Verificar se o e-mail foi alterado e se é novo e válido
            if (existingSupplier.Email != model.Email)
            {
                var emailExists = await _suppliersRepository.VerificarEmailExistente2(model.Email, id);
                if (emailExists)
                {
                    return new ServiceResponse<SuppliersResponse>("E-mail duplicado.");
                }
            }

            // Convert the 'Uf' value to uppercase before validation and saving
            if (!string.IsNullOrEmpty(model.Uf))
            {
                model.Uf = model.Uf.ToUpper();
            }

            // Verificar se estado corresponde a uma das 27 siglas de estados brasileiros
            var estadosBrasileiros = new[] { "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO" };
            if (!string.IsNullOrEmpty(model.Uf) && !estadosBrasileiros.Contains(model.Uf, StringComparer.OrdinalIgnoreCase))
            {
                return new ServiceResponse<SuppliersResponse>("Estado inválido.");
            }

            // Atualizar os campos do fornecedor com os valores do modelo, se eles não forem nulos ou vazios
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                existingSupplier.Email = model.Email;
            }

            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                existingSupplier.Phone = model.Phone;
            }

            if (!string.IsNullOrWhiteSpace(model.City))
            {
                existingSupplier.City = model.City;
            }

            if (!string.IsNullOrWhiteSpace(model.Uf))
            {
                existingSupplier.Uf = model.Uf;
            }

            if (!string.IsNullOrWhiteSpace(model.ContactName))
            {
                existingSupplier.ContactName = model.ContactName;
            }

            // Chamar o método Editar do repositório para salvar as alterações no banco de dados
            var updatedSupplier = await _suppliersRepository.Editar(existingSupplier);

            // Verificar se a edição foi realizada com sucesso
            if (updatedSupplier == null)
            {
                return new ServiceResponse<SuppliersResponse>("Erro ao editar fornecedor.");
            }

            // Criar o objeto SuppliersResponse para retornar ao cliente
            var response = new SuppliersResponse(updatedSupplier);

            return new ServiceResponse<SuppliersResponse>(response);
        }


    }
}
