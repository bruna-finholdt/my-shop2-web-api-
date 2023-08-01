using Microsoft.AspNetCore.Identity;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services.Base;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Minishop.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersService(IUsersRepository usersRepository, IHttpContextAccessor httpContextAccessor)
        {
            _usersRepository = usersRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<UserResponse>> Logar(UserLoginRequest model)
        {
            //var hasher = new PasswordHasher<User>();
            var user = await _usersRepository.PesquisaPorNome(model.UserName);

            if (user == null)
            {
                return new ServiceResponse<UserResponse>("Usuário ou senha incorretos");
            }

            var hasher = new PasswordHasher<User>();
            var verificação = hasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (verificação == PasswordVerificationResult.Failed)
            {
                return new ServiceResponse<UserResponse>("Usuário ou senha incorretos");
            }

            return new ServiceResponse<UserResponse>(new UserResponse(user));
        }

        public async Task<ServicePagedResponse<UserResponse>> Pesquisar(PageQueryRequest queryResquest)
        {
            //Lista Users com paginação
            {
                // Consulta itens no banco
                var listaPesquisa = await _usersRepository.Pesquisar(
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
                // Conta itens do banco
                var contagem = await _usersRepository.Contagem();
                // Transforma User em UserResponse
                var listaConvertida = listaPesquisa
                    .Select(user => new UserResponse(user));

                // Cria resultado com paginação
                return new ServicePagedResponse<UserResponse>(
                    listaConvertida,
                    contagem,
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
            }
        }
        public async Task<ServiceResponse<UserResponse>> PesquisaPorId(int id)
        {
            var user = await _usersRepository.PesquisaPorId(id);
            if (user == null)
            {
                return new ServiceResponse<UserResponse>(
                    "User não encontrado"
                );
            }

            return new ServiceResponse<UserResponse>(
                new UserResponse(user)
            );

        }
        public async Task<ServiceResponse<UserResponse>> PesquisaPorNome(string userName)
        {
            var user = await _usersRepository.PesquisaPorNome(userName);

            if (user == null)
            {
                return new ServiceResponse<UserResponse>("Usuário não encontrado.");
            }

            return new ServiceResponse<UserResponse>(new UserResponse(user));
        }



        public async Task<ServiceResponse<UserResponse>> Cadastrar(UserCreateRequest model)
        {
            // Verificar se o username já está em uso 
            var usernameExists = await _usersRepository.VerificarUsernameExistente(model.UserName);
            if (usernameExists)
            {
                return new ServiceResponse<UserResponse>("User já cadastrado.");
            }


            if (model.Password != model.PasswordConfirmation)
            {
                return new ServiceResponse<UserResponse>("A confirmação de senha não corresponde à senha digitada.");
            }

            // Obter a lista de RoleIds válidos do repositório
            var validRoleIds = await _usersRepository.GetValidRoleIds();

            // Verificar se o RoleId é válido
            if (!validRoleIds.Contains(model.RoleId))
            {
                var validRoleIdsStr = string.Join(", ", validRoleIds);
                return new ServiceResponse<UserResponse>($"RoleId inválido! O RoleId do usuário deve ser um dos seguintes valores: {validRoleIdsStr}.");
            }

            // Mapear o UserCreateRequest para a entidade User
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                RoleId = model.RoleId,
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(model.Password)), // Codificar a senha em Base64 antes de salvar
                //PasswordConfirmation = model.PasswordConfirmation
            };

            // Gerar o hash seguro da senha antes de salvar no banco de dados
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, model.Password);

            // Chamar o método Cadastrar do repositório para salvar o novo user no banco de dados
            var createdUser = await _usersRepository.Cadastrar(user);

            // Verificar se o cadastro foi realizado com sucesso
            if (createdUser == null)
            {
                return new ServiceResponse<UserResponse>("Erro ao cadastrar user.");
            }

            // Criar o objeto UserResponse para retornar ao user
            var response = new UserResponse(createdUser);

            return new ServiceResponse<UserResponse>(response);
        }

        public async Task<ServiceResponse<UserResponse>> Editar(int id, UserUpdateRequest model)
        {

            // Verificar se o user com o ID fornecido existe no banco de dados
            var existingUser = await _usersRepository.PesquisaPorId(id);
            if (existingUser == null)
            {
                return new ServiceResponse<UserResponse>("User não encontrado.");
            }

            var user = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault()!.Value;

            // Verificar se o usuário logado está tentando editar o seu próprio perfil
            if (user == existingUser.UserName)
            {
                return new ServiceResponse<UserResponse>("Não é permitido editar o próprio perfil.");
            }

            // Obter a lista de RoleIds válidos do repositório
            var validRoleIds = await _usersRepository.GetValidRoleIds();

            // Verificar se o RoleId é válido, mas permitir que seja null (não modificado)
            if (model.RoleId.HasValue && !validRoleIds.Contains(model.RoleId.Value))
            {
                var validRoleIdsStr = string.Join(", ", validRoleIds);
                return new ServiceResponse<UserResponse>($"RoleId inválido! O RoleId do usuário deve ser um dos seguintes valores: {validRoleIdsStr}.");
            }

            // Atualizar os campos do cliente com os valores do modelo, se eles não forem nulos ou vazios
            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                existingUser.FirstName = model.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                existingUser.LastName = model.LastName;
            }

            if (!string.IsNullOrWhiteSpace(model.UserName))
            {
                existingUser.UserName = model.UserName;
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                existingUser.Password = model.Password;
            }

            if (model.RoleId.HasValue)
            {
                existingUser.RoleId = model.RoleId.Value;
            }

            // Chamar o método Editar do repositório para salvar as alterações no banco de dados
            var updatedUser = await _usersRepository.Editar(existingUser);

            // Verificar se a edição foi realizada com sucesso
            if (updatedUser == null)
            {
                return new ServiceResponse<UserResponse>("Erro ao editar user.");
            }

            // Criar o objeto CustomerResponse para retornar ao cliente
            var response = new UserResponse(updatedUser);

            return new ServiceResponse<UserResponse>(response);
        }

    }
}
