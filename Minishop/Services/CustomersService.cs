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
        private readonly ICustomersRepository _customersRepository;

        public CustomersService(ICustomersRepository customersRepository)
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


        public async Task<ServiceResponse<CustomerResponse>> Cadastrar(CustomerCreateRequest model)
        {
            // Verificar se o CPF já está em uso (opcional, dependendo dos requisitos)
            var cpfExists = await _customersRepository.VerificarCpfExistente(model.Cpf);
            if (cpfExists)
            {
                return new ServiceResponse<CustomerResponse>("CPF já cadastrado.");
            }

            // Verificar se o e-mail já está em uso (opcional, dependendo dos requisitos)
            var emailExists = await _customersRepository.VerificarEmailExistente(model.Email);
            if (emailExists)
            {
                return new ServiceResponse<CustomerResponse>("E-mail já cadastrado.");
            }

            // Mapear o CustomerCreateRequest para a entidade Customer
            var customer = new Customer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Cpf = model.Cpf
                // Preencha outras propriedades da entidade, se houver
            };

            // Chamar o método Cadastrar do repositório para salvar o novo cliente no banco de dados
            var createdCustomer = await _customersRepository.Cadastrar(customer);

            // Verificar se o cadastro foi realizado com sucesso
            if (createdCustomer == null)
            {
                return new ServiceResponse<CustomerResponse>("Erro ao cadastrar cliente.");
            }

            // Criar o objeto CustomerResponse para retornar ao cliente
            var response = new CustomerResponse(createdCustomer);

            return new ServiceResponse<CustomerResponse>(response);
        }

        public async Task<ServiceResponse<CustomerResponse>> Editar(int id, CustomerUpdateRequest model)
        {
            // Verificar se o cliente com o ID fornecido existe no banco de dados
            var existingCustomer = await _customersRepository.PesquisaPorId(id);
            if (existingCustomer == null)
            {
                return new ServiceResponse<CustomerResponse>("Cliente não encontrado.");
            }


            // Verificar se o e-mail é novo e válido (não duplicado)
            var newEmailExists = await _customersRepository.VerificarNovoEmailExistente(model.Email, id);
            if (newEmailExists)
            {
                return new ServiceResponse<CustomerResponse>("E-mail duplicado.");
            }

            // Atualizar os campos do cliente com os valores do modelo, se eles não forem nulos ou vazios
            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                existingCustomer.FirstName = model.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                existingCustomer.LastName = model.LastName;
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                existingCustomer.Email = model.Email;
            }

            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                existingCustomer.Phone = model.Phone;
            }

            // Chamar o método Editar do repositório para salvar as alterações no banco de dados
            var updatedCustomer = await _customersRepository.Editar(existingCustomer);

            // Verificar se a edição foi realizada com sucesso
            if (updatedCustomer == null)
            {
                return new ServiceResponse<CustomerResponse>("Erro ao editar cliente.");
            }

            // Criar o objeto CustomerResponse para retornar ao cliente
            var response = new CustomerResponse(updatedCustomer);

            return new ServiceResponse<CustomerResponse>(response);
        }


    }
}
