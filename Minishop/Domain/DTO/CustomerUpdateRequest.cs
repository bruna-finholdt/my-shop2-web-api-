﻿using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class CustomerUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }


    }
}
