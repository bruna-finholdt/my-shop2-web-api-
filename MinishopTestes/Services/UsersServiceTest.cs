using Microsoft.AspNetCore.Identity;
using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minishop.Tests.Services
{
    public class UsersServiceTest
    {
        private readonly Mock<IUsersRepository> _mockRepository;
        private readonly UsersService _service;

        public UsersServiceTest()
        {
            _mockRepository = new Mock<IUsersRepository>();
            _service = new UsersService(_mockRepository.Object);
        }

        [Fact]
        public async Task Logar_DeveRetornarUsuarioQuandoLoginForValido()
        {
            //Arrange
            var loginRequest = new UserLoginRequest
            {
                UserName = "bru",
                Password = "admin123"
            };

            var hashedPassword = new PasswordHasher<User>().HashPassword(new User(), loginRequest.Password);

            var user = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = hashedPassword
            };

            _mockRepository.Setup(repo => repo.PesquisaPorNome(loginRequest.UserName))
                .ReturnsAsync(user);

            //Act
            var result = await _service.Logar(loginRequest);

            //Assert
            Assert.True(result.Sucesso);
            Assert.NotNull(result.Conteudo);
            Assert.Equal(user.Id, result.Conteudo.Id);
        }

        [Fact]
        public async Task Logar_DeveRetornarErroQuandoUsuarioNaoExiste()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                UserName = "userNaoExistente",
                Password = "123456"
            };

            ////Simula usuário não encontrado no db
            //_mockRepository.Setup(repo => repo.PesquisaPorNome(loginRequest.UserName))
            //    .ReturnsAsync((User)null);

            //Act
            var result = await _service.Logar(loginRequest);

            //Assert
            Assert.False(result.Sucesso);
            Assert.Null(result.Conteudo);
            Assert.Equal("Usuário ou senha incorretos", result.Mensagem);
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarListaPaginadaDeUsuarios()
        {
            // Arrange
            var pageQueryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 10
            };

            var usersList = new List<User>
            {
                new User { Id = 1, FirstName = "Bruna", LastName = "Perez", UserName = "bru", RoleId = 1, Password = "admin123" },
                new User { Id = 2, FirstName = "Julia", LastName = "Perez", UserName = "ju", RoleId = 2, Password = "123456" },
            };

            var usersResponseList = usersList.Select(user => new UserResponse(user)).ToList();

            _mockRepository.Setup(repo => repo.Pesquisar(pageQueryRequest.PaginaAtual, pageQueryRequest.Quantidade))
                .ReturnsAsync(usersList);

            //Act
            var result = await _service.Pesquisar(pageQueryRequest);

            //Assert
            Assert.True(result.Sucesso);
            Assert.Equivalent(usersResponseList, result.Conteudo);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarUsuarioQuandoEncontrado()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = "admin123"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _service.PesquisaPorId(userId);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(user.Id, result.Conteudo.Id);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarErroQuandoUsuarioNaoEncontrado()
        {
            //Arrange
            var userId = 99;

            _mockRepository.Setup(repo => repo.PesquisaPorId(userId))
                .ReturnsAsync((User)null);

            //Act
            var result = await _service.PesquisaPorId(userId);

            //Assert
            Assert.False(result.Sucesso);
            Assert.Null(result.Conteudo);
            Assert.Equal("User não encontrado", result.Mensagem);
        }

        [Fact]
        public async Task PesquisaPorNome_DeveRetornarUsuarioQuandoEncontrado()
        {
            //Arrange
            var userName = "bru";
            var user = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = userName,
                RoleId = 1,
                Password = "admin123"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorNome(userName))
                .ReturnsAsync(user);

            //Act
            var result = await _service.PesquisaPorNome(userName);

            //Assert
            Assert.True(result.Sucesso);
            Assert.Equal(user.Id, result.Conteudo.Id);
        }

        [Fact]
        public async Task PesquisaPorNome_DeveRetornarErroQuandoUsuarioNaoEncontrado()
        {
            // Arrange
            var userName = "bru";

            _mockRepository.Setup(repo => repo.PesquisaPorNome(userName))
                .ReturnsAsync((User)null);

            // Act
            var result = await _service.PesquisaPorNome(userName);

            // Assert
            Assert.False(result.Sucesso);
            Assert.Null(result.Conteudo);
            Assert.Equal("Usuário não encontrado.", result.Mensagem);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarUsuarioCriadoQuandoCadastroForValido()
        {
            // Arrange
            var userCreateRequest = new UserCreateRequest
            {
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = "admin123",
                PasswordConfirmation = "admin123"
            };

            _mockRepository.Setup(repo => repo.VerificarUsernameExistente(userCreateRequest.UserName))
                .ReturnsAsync(false);//userName existente?

            _mockRepository.Setup(repo => repo.GetValidRoleIds())//Id válido?
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            _mockRepository.Setup(repo => repo.Cadastrar(It.IsAny<User>()))
                .ReturnsAsync((User newUser) => newUser);

            // Act
            var result = await _service.Cadastrar(userCreateRequest);

            // Assert
            Assert.True(result.Sucesso);
            Assert.NotNull(result.Conteudo);
        }

        [Fact]
        public async Task Cadastrar_DeveRetornarErroQuandoRoleIdInvalido()
        {
            //Arrange
            var userCreateRequest = new UserCreateRequest
            {
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 5, // RoleId inválido
                Password = "123456",
                PasswordConfirmation = "123456"
            };

            _mockRepository.Setup(repo => repo.VerificarUsernameExistente(userCreateRequest.UserName))
                .ReturnsAsync(false);

            _mockRepository.Setup(repo => repo.GetValidRoleIds())
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            //Act
            var result = await _service.Cadastrar(userCreateRequest);

            //Assert
            Assert.False(result.Sucesso);
            Assert.Null(result.Conteudo);
            Assert.Equal("RoleId inválido! O RoleId do usuário deve ser um dos seguintes valores: 1, 2, 3.", result.Mensagem);
        }

        [Fact]
        public async Task Editar_DeveRetornarUsuarioEditadoQuandoEdicaoForValida()
        {
            // Arrange
            var userId = 1;
            var userUpdateRequest = new UserUpdateRequest
            {
                FirstName = "Updated",
                LastName = "User",
                UserName = "updated_user",
                Password = "newpassword",
                RoleId = 2
            };

            var existingUser = new User
            {
                Id = userId,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = "admin123"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(userId))
                .ReturnsAsync(existingUser);

            _mockRepository.Setup(repo => repo.GetValidRoleIds())
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            _mockRepository.Setup(repo => repo.Editar(It.IsAny<User>()))
                .ReturnsAsync((User updatedUser) => updatedUser);

            //Act
            var result = await _service.Editar(userId, userUpdateRequest, "not_the_same_user");

            //Assert
            Assert.True(result.Sucesso);
            Assert.NotNull(result.Conteudo);
            Assert.Equal(userUpdateRequest.FirstName, result.Conteudo.FirstName);
            Assert.Equal(userUpdateRequest.LastName, result.Conteudo.LastName);
            Assert.Equal(userUpdateRequest.UserName, result.Conteudo.UserName);
            Assert.Equal(userUpdateRequest.RoleId, result.Conteudo.RoleId);
        }

        [Fact]
        public async Task Editar_DeveRetornarErroQuandoUsuarioTentaEditarSeuProprioPerfil()
        {
            //Arrange
            var userId = 1;
            var userUpdateRequest = new UserUpdateRequest
            {
                FirstName = "Updated",
                LastName = "User",
                UserName = "updated_user",
                Password = "newpassword",
                RoleId = 2
            };

            var existingUser = new User
            {
                Id = userId,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",//user tentando editar seu próprio perfil
                RoleId = 1,
                Password = "admin123"
            };

            _mockRepository.Setup(repo => repo.PesquisaPorId(userId))
                .ReturnsAsync(existingUser);

            _mockRepository.Setup(repo => repo.GetValidRoleIds())
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            // Act
            var result = await _service.Editar(userId, userUpdateRequest, "bru");//user tentando editar seu próprio perfil

            // Assert
            Assert.False(result.Sucesso);
            Assert.Null(result.Conteudo);
            Assert.Equal("Não é permitido editar o próprio perfil.", result.Mensagem);
        }
    }
}

