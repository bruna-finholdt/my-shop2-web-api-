﻿using Minishop.Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace Minishop.Domain.DTO
{
    public class ProductCreateRequest
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [MinLength(10, ErrorMessage = "O tamanho mínimo de caracteres no nome do produto é 10")]
        [StringLength(100, ErrorMessage = "Tamanho máximo de caracteres no nome do produto é 100")]
        public string ProductName { get; set; } = null!;
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public decimal? UnitPrice { get; set; }
        [Required]
        public bool IsDiscontinued { get; set; }

        [StringLength(100, ErrorMessage = "Tamanho máximo de caracteres no nome do produto é 100")]
        public string? PackageName { get; set; }
    }
}
