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
        private readonly SuppliersRepository _suppliersRepository;
        private readonly Minishop2023Context _context;
        public SuppliersService(SuppliersRepository suppliersRepository, Minishop2023Context context)
        {
            _suppliersRepository = suppliersRepository;
            _context = context;
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

            //var products = await _productsRepository.PesquisarSupplierId(id);

            return new ServiceResponse<SuppliersCompletoResponse>(
                new SuppliersCompletoResponse(supplier)
            );
            //pra pesquisa de customer por id, usa-se o CustomerCompletoResponse (com tds as informações)
        }

        public async Task<ServiceResponse<SuppliersResponse>> Cadastrar(SupplierCreateRequest novo)
        {

            // Verificar se é um CNPJ novo e válido
            if (_context.Suppliers.Any(s => s.Cnpj == novo.Cnpj))
            {
                return new ServiceResponse<SuppliersResponse>("CNPJ duplicado");
            }

            // Verificar se é um e-mail novo e válido
            if (_context.Suppliers.Any(s => s.Email == novo.Email))
            {
                return new ServiceResponse<SuppliersResponse>("E-mail duplicado");
            }

            // Verificar se estado corresponde a uma das 27 siglas de estados brasileiros
            var estadosBrasileiros = new[] { "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO" };
            if (!estadosBrasileiros.Contains(novo.Uf))
            {
                return new ServiceResponse<SuppliersResponse>("Estado inválido");
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

            await _suppliersRepository.Cadastrar(supplier);

            return new ServiceResponse<SuppliersResponse>(new SuppliersResponse(supplier));
        }

        public async Task<ServiceResponse<Supplier>> Editar(int id, SupplierUpdateRequest model)
        {
            var resultado = _context.Suppliers.FirstOrDefault(x => x.Id == id);

            if (resultado == null)
            {
                return new ServiceResponse<Supplier>("Fornecedor não encontrado");
            }

            // Verificar se é um e-mail novo e válido
            if (_context.Suppliers.Any(s => s.Email == model.Email))
            {
                return new ServiceResponse<Supplier>("E-mail duplicado");
            }

            // Verificar se estado corresponde a uma das 27 siglas de estados brasileiros
            var estadosBrasileiros = new[] { "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO" };
            if (!estadosBrasileiros.Contains(model.Uf))
            {
                return new ServiceResponse<Supplier>("Estado inválido");
            }

            resultado.Email = model.Email;
            resultado.City = model.City;
            resultado.Uf = model.Uf;
            resultado.Phone = model.Phone;
            resultado.ContactName = model.ContactName;

            _context.Entry(resultado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ServiceResponse<Supplier>(resultado);
        }

    }
}
