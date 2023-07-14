using k8s.KubeConfigModels;
using Microsoft.EntityFrameworkCore;
using Minishop.DAL;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class CustomersService : ICustomersService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly CustomersRepository _customersRepository;

        private readonly Minishop2023Context _context;

        public CustomersService(CustomersRepository customersRepository, Minishop2023Context context)
        {
            _customersRepository = customersRepository;
            _context = context;

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
            //funcionam como um transformador para cada objeto da lista
        }
        public async Task<ServiceResponse<CustomerCompletoResponse>> PesquisaPorId(int id)
        //Para usar um método async, devemos colocar async na assinatura, Task<> no retorno e colocar o
        //await na chamada de qualquer método async interno.
        {
            var customer = await _customersRepository.PesquisaPorId(id);
            if (customer == null)
            {
                return new ServiceResponse<CustomerCompletoResponse>(
                    "Cliente não encontrado"
                );
            }

            //var customerOrders = await _ordersRepository.PesquisarCustomerId(id);

            return new ServiceResponse<CustomerCompletoResponse>(
                new CustomerCompletoResponse(customer)
            );
            //pra pesquisa de customer por id, usa-se o CustomerCompletoResponse (com tds as informações)
        }

        public async Task<ServiceResponse<CustomerResponse>> Cadastrar(CustomerCreateRequest novo)
        {

            // Verificar se é um CNPJ novo e válido
            if (_context.Customers.Any(s => s.Cpf == novo.Cpf))
            {
                return new ServiceResponse<CustomerResponse>("CPF duplicado");
            }

            // Verificar se é um e-mail novo e válido
            if (_context.Customers.Any(s => s.Email == novo.Email))
            {
                return new ServiceResponse<CustomerResponse>("E-mail duplicado");
            }

            var produto = new Customer()
            {
                FirstName = novo.FirstName,
                LastName = novo.LastName,
                Email = novo.Email,
                Phone = novo.Phone,
                Cpf = novo.Cpf
            };

            await _customersRepository.Cadastrar(produto);

            return new ServiceResponse<CustomerResponse>(new CustomerResponse(produto));
        }

        public async Task<ServiceResponse<Customer>> Editar(int id, CustomerUpdateRequest model)
        {
            var resultado = _context.Customers.FirstOrDefault(x => x.Id == id);

            if (resultado == null)
            {
                return new ServiceResponse<Customer>("Cliente não encontrado");
            }

            // Verificar se é um e-mail novo e válido
            if (_context.Customers.Any(s => s.Email == model.Email))
            {
                return new ServiceResponse<Customer>("E-mail duplicado");
            }

            resultado.Email = model.Email;
            resultado.Phone = model.Phone;

            _context.Entry(resultado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ServiceResponse<Customer>(resultado);
        }
    }
}
