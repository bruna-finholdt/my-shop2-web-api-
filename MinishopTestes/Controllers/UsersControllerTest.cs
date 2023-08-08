using Microsoft.AspNetCore.Mvc;
using Minishop.Controllers;
using Minishop.Domain.DTO;
using Minishop.Domain.Entity;
using Minishop.Services;
using Minishop.Services.Base;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minishop.Tests.Controllers
{
    public class UsersControllerTest
    {
        private readonly Mock<IUsersService> _mockService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            _mockService = new Mock<IUsersService>();
            _mockTokenService = new Mock<ITokenService>();
            _controller = new UsersController(_mockService.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Get_DeveRetornarOkQuandoServicoRetornarSucesso()
        {
            //Arrange
            var queryRequest = new PageQueryRequest
            {
                PaginaAtual = 1,
                Quantidade = 10
            };

            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Bruna",
                    LastName = "Perez",
                    UserName = "bru",
                    RoleId = 1,
                    Role = new Role { Id = 1, RoleName = "admin" }
                },
                new User
                {
                    Id = 2,
                    FirstName = "Julia",
                    LastName = "Perez",
                    UserName = "ju",
                    RoleId = 2,
                    Role = new Role { Id = 2, RoleName = "common" }
                }
            };

            var response = new ServicePagedResponse<UserResponse>(
                users.ConvertAll(u => new UserResponse(u)),
                2,//Total de usuários na página
                1,//Página atual
                10//Tamanho da página
            );

            _mockService.Setup(service => service.Pesquisar(queryRequest))
                .ReturnsAsync(response);

            //Act
            var result = await _controller.Get(queryRequest);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarOkQuandoServicoRetornarUsuario()
        {
            //Arrange
            int id = 1;

            var user = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Role = new Role { Id = 1, RoleName = "admin" }
            };

            var response = new ServiceResponse<UserResponse>(
                new UserResponse(user)
            );

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(response);

            //Act
            var result = await _controller.GetById(id.ToString());

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFoundQuandoIdForInvalido()
        {
            // Arrange
            int id = 999;//Id inválido

            var mensagem = "Usuário não encontrado";

            var retorno = new ServiceResponse<UserResponse>(mensagem);

            _mockService.Setup(service => service.PesquisaPorId(id))
                .ReturnsAsync(retorno);

            //Act
            var result = await _controller.GetById(id.ToString());

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<UserResponse>>(notFoundResult.Value);

            Assert.False(response.Sucesso);
            Assert.Equal(mensagem, response.Mensagem);
        }

        //LOGIN DE USER
        [Fact]
        public async Task AuthenticateAsync_DeveRetornarOkQuandoUsuarioAutenticado()
        {
            //Arrange
            var loginRequest = new UserLoginRequest
            {
                UserName = "bru",
                Password = "admin123"
            };

            var user = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Role = new Role { Id = 1, RoleName = "admin" }
            };

            var loginResponse = new ServiceResponse<UserResponse>(new UserResponse(user));
            var token = "31F0E44478144E93A9EFA9115555A8ED";
            loginResponse.Token = token;

            _mockService.Setup(service => service.Logar(loginRequest))
                .ReturnsAsync(loginResponse);

            _mockTokenService.Setup(service => service.GenerateToken(loginResponse.Conteudo))
                .Returns(token);

            //Act
            var result = await _controller.AuthenticateAsync(loginRequest);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var response = okResult.Value as ServiceResponse<UserResponse>;
            Assert.NotNull(response);
            Assert.True(response.Sucesso);
            Assert.NotNull(response.Conteudo);
            Assert.Equal(user.Id, response.Conteudo.Id);
            Assert.Equal(user.FirstName, response.Conteudo.FirstName);
            Assert.Equal(user.LastName, response.Conteudo.LastName);
            Assert.Equal(user.UserName, response.Conteudo.UserName);
            Assert.Equal(user.RoleId, response.Conteudo.RoleId);
            Assert.NotNull(response.Token);
            Assert.Equal(token, response.Token);
        }

        [Fact]
        public async Task AuthenticateAsync_DeveRetornarBadRequestQuandoAutenticacaoFalha()
        {
            // Arrange
            var loginRequest = new UserLoginRequest
            {
                UserName = "bru",
                Password = "123456"
            };

            //Simula um ServiceResponse indicando erro de autenticação
            var errorMessage = "Usuário ou senha incorretos";
            var errorResponse = new ServiceResponse<UserResponse>(errorMessage);

            _mockService.Setup(service => service.Logar(loginRequest))
                .ReturnsAsync(errorResponse);

            //Act
            var result = await _controller.AuthenticateAsync(loginRequest);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            var response = badRequestResult.Value as ServiceResponse<UserResponse>;
            Assert.NotNull(response);
            Assert.False(response.Sucesso);
            Assert.Null(response.Conteudo);
            Assert.Equal(errorMessage, response.Mensagem);
        }

        //CADASTRO DE USER
        [Fact]
        public async Task Post_DeveRetornarOkQuandoServicoRetornarUsuarioCriado()
        {
            //Arrange
            var createRequest = new UserCreateRequest
            {
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                Password = "admin123",
                PasswordConfirmation = "admin123",
                RoleId = 1
            };

            var createdUser = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Role = new Role { Id = 1, RoleName = "admin" }
            };

            var response = new ServiceResponse<UserResponse>(
                new UserResponse(createdUser)
            );

            _mockService.Setup(service => service.Cadastrar(createRequest))
                .ReturnsAsync(response);

            //Act
            var result = await _controller.Post(createRequest);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(response.Conteudo, okResult.Value);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequestQuandoServicoRetornarErro()
        {
            // Arrange
            var createRequest = new UserCreateRequest
            {
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                Password = "admin123",
                PasswordConfirmation = "admin123",
                RoleId = 1
            };

            var errorResponse = new ServiceResponse<UserResponse>("Erro ao cadastrar o usuário");

            _mockService.Setup(service => service.Cadastrar(createRequest))
                .ReturnsAsync(errorResponse);

            //Act
            var result = await _controller.Post(createRequest);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.Equal(errorResponse, badRequestResult.Value);
        }

        [Fact]
        public async Task Put_DeveRetornarOkQuandoServicoRetornarUsuarioAtualizado()
        {
            // Arrange
            int userId = 1;

            var updateRequest = new UserUpdateRequest
            {
                UserName = "bru_updated",
            };

            var updatedUser = new User
            {
                Id = userId,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru_updated",
                RoleId = 1,
                Role = new Role { Id = 1, RoleName = "admin" }
            };

            var response = new ServiceResponse<UserResponse>(
                new UserResponse(updatedUser)
            );

            _mockService.Setup(service => service.Editar(userId, updateRequest, It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Put(userId, updateRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(userId, responseData.Id);
            Assert.Equal(updateRequest.UserName, responseData.UserName);
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequestQuandoServicoRetornarErro()
        {
            //Arrange
            int userId = 1;

            var updateRequest = new UserUpdateRequest
            {
                UserName = "bru_updated",
            };

            var errorMessage = "Erro ao editar o usuário.";

            var response = new ServiceResponse<UserResponse>(errorMessage);

            _mockService.Setup(service => service.Editar(userId, updateRequest, It.IsAny<string>()))
                .ReturnsAsync(response);

            //Act
            var result = await _controller.Put(userId, updateRequest);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseData = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal(errorMessage, responseData);
        }

    }
}
