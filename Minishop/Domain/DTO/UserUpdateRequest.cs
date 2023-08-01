using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class UserUpdateRequest
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public int? RoleId { get; set; }
    }
}
