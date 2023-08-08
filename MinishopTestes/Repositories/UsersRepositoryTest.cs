using Microsoft.EntityFrameworkCore;
using Minishop.DAL.Repositories;
using Minishop.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minishop.Domain.Entity;

namespace Minishop.Tests.Repositories
{
    public class UsersRepositoryTest : IDisposable
    {
        private readonly Minishop2023Context _dbContext;
        private readonly UsersRepository _userRepository;

        public UsersRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Minishop2023Context>()
                .UseInMemoryDatabase(databaseName: "TestUserDatabase")
                .Options;

            _dbContext = new Minishop2023Context(dbContextOptions);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Roles.Add(new Role { Id = 1, RoleName = "admin" });
            _dbContext.Roles.Add(new Role { Id = 2, RoleName = "common" });
            _dbContext.Roles.Add(new Role { Id = 3, RoleName = "seller" });
            _dbContext.SaveChanges();
            _userRepository = new UsersRepository(_dbContext);
        }

        [Fact]
        public async Task Pesquisar_DeveRetornarListagem()
        {
            int paginaAtual = 1;
            int qtdPagina = 10;

            var users = new List<User>
            {
                new User { Id = 1, FirstName = "Julia", LastName = "Perez", UserName = "ju", Password = "123456", RoleId = 2 },
                new User { Id = 2, FirstName = "Bruna", LastName = "Perez", UserName = "bru", Password = "admin123", RoleId = 1 }
            };

            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();

            var result = await _userRepository.Pesquisar(paginaAtual, qtdPagina);

            Assert.Equal(users.Count, result.Count);
            Assert.Equal("Bruna", result[0].FirstName);
            Assert.Equal("Julia", result[1].FirstName);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarUsuarioCorreto()
        {
            //Arrange
            var user1 = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = "admin123"
            };

            var user2 = new User
            {
                Id = 2,
                FirstName = "Julia",
                LastName = "Perez",
                UserName = "ju",
                RoleId = 2,
                Password = "123456"
            };

            _dbContext.Users.AddRange(user1, user2);
            _dbContext.SaveChanges();

            //Act
            var result1 = await _userRepository.PesquisaPorId(user1.Id);
            var result2 = await _userRepository.PesquisaPorId(user2.Id);

            //Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(user1.FirstName, result1.FirstName);
            Assert.Equal(user1.LastName, result1.LastName);
            Assert.Equal(user1.UserName, result1.UserName);
            Assert.Equal(user1.RoleId, result1.RoleId);
            Assert.Equal(user2.FirstName, result2.FirstName);
            Assert.Equal(user2.LastName, result2.LastName);
            Assert.Equal(user2.UserName, result2.UserName);
            Assert.Equal(user2.RoleId, result2.RoleId);
        }

        [Fact]
        public async Task PesquisaPorId_DeveRetornarNullQuandoUsuarioNaoExiste()
        {
            //Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = "admin123"
            };

            _dbContext.Users.AddRange(user);
            _dbContext.SaveChanges();

            int idNaoExistente = 5;

            //Act
            var result = await _userRepository.PesquisaPorId(idNaoExistente);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetValidRoleIds_DeveRetornarListaDeIdsDePapeisValidos()
        {
            //Arrange
            //Já foi realizado no construtor

            //Act
            var result = await _userRepository.GetValidRoleIds();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(2, result);
            Assert.Contains(3, result);
        }

        [Fact]
        public async Task PesquisaPorNome_DeveRetornarUsuarioCorreto()
        {
            // Arrange
            var user1 = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = "admin123"
            };

            var user2 = new User
            {
                Id = 2,
                FirstName = "Julia",
                LastName = "Perez",
                UserName = "ju",
                RoleId = 2,
                Password = "123456"
            };

            _dbContext.Users.AddRange(user1, user2);
            _dbContext.SaveChanges();

            // Act
            var result1 = await _userRepository.PesquisaPorNome(user1.UserName);
            var result2 = await _userRepository.PesquisaPorNome(user2.UserName);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(user1.FirstName, result1.FirstName);
            Assert.Equal(user1.LastName, result1.LastName);
            Assert.Equal(user1.UserName, result1.UserName);
            Assert.Equal(user1.RoleId, result1.RoleId);
            Assert.Equal(user2.FirstName, result2.FirstName);
            Assert.Equal(user2.LastName, result2.LastName);
            Assert.Equal(user2.UserName, result2.UserName);
            Assert.Equal(user2.RoleId, result2.RoleId);
        }

        [Fact]
        public async Task PesquisaPorNome_DeveRetornarNullQuandoUsuarioNaoExiste()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Bruna",
                LastName = "Perez",
                UserName = "bru",
                RoleId = 1,
                Password = "admin123"
            };

            _dbContext.Users.AddRange(user);
            _dbContext.SaveChanges();

            string userNameInexistente = "userName_inexistente";

            // Act
            var result = await _userRepository.PesquisaPorNome(userNameInexistente);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Cadastrar_DeveAdicionarNovoUsuario()
        {
            // Arrange
            var newUser = new User
            {
                Id = 1,
                FirstName = "New",
                LastName = "User",
                UserName = "new_user",
                RoleId = 1,
                Password = "123456"
            };

            // Act
            var result = await _userRepository.Cadastrar(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.FirstName, result.FirstName);
            Assert.Equal(newUser.LastName, result.LastName);
            Assert.Equal(newUser.UserName, result.UserName);
            Assert.Equal(newUser.RoleId, result.RoleId);
            var userInDatabase = await _dbContext.Users.FindAsync(result.Id);
            Assert.NotNull(userInDatabase);
            Assert.Equal(newUser.FirstName, userInDatabase.FirstName);
            Assert.Equal(newUser.LastName, userInDatabase.LastName);
            Assert.Equal(newUser.UserName, userInDatabase.UserName);
            Assert.Equal(newUser.RoleId, userInDatabase.RoleId);
        }

        [Fact]
        public async Task Editar_DeveAtualizarUsuarioExistente()
        {
            //Arrange
            var existingUser = new User
            {
                Id = 1,
                FirstName = "Existing",
                LastName = "User",
                UserName = "existing_user",
                RoleId = 1,
                Password = "123456"
            };

            _dbContext.Users.Add(existingUser);
            _dbContext.SaveChanges();

            existingUser.FirstName = "Updated";
            existingUser.LastName = "User";
            existingUser.UserName = "updated_user";
            existingUser.RoleId = 2;

            //Act
            var result = await _userRepository.Editar(existingUser);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.FirstName, result.FirstName);
            Assert.Equal(existingUser.LastName, result.LastName);
            Assert.Equal(existingUser.UserName, result.UserName);
            Assert.Equal(existingUser.RoleId, result.RoleId);
            var userInDatabase = await _dbContext.Users.FindAsync(result.Id);
            Assert.NotNull(userInDatabase);
            Assert.Equal(existingUser.FirstName, userInDatabase.FirstName);
            Assert.Equal(existingUser.LastName, userInDatabase.LastName);
            Assert.Equal(existingUser.UserName, userInDatabase.UserName);
            Assert.Equal(existingUser.RoleId, userInDatabase.RoleId);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
