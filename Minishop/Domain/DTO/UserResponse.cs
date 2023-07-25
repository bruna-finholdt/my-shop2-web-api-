using Minishop.Domain.Entity;

namespace Minishop.Domain.DTO
{
    public class UserResponse
    {
        public UserResponse(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName;
            RoleId = user.RoleId;
            if (user.Role != null)
            {
                RoleName = user.Role.RoleName; // Carrega o nome do Role a partir da propriedade de navegação Role

            };
        }

        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserName { get; private set; }
        public int RoleId { get; private set; }
        public string RoleName { get; private set; } = null!;
    }
}

