using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Minishop.Domain.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; } //foreign key for Role

        [JsonIgnore] //info sensível - não deve estar presente no json
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
