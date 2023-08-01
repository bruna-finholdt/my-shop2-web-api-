using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minishop.Domain.Entity;

public partial class Product
{
    [Key]
    /// <summary>
    /// Chave primária (identificador) de cada registro da tabela
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do produto
    /// </summary>
    public string ProductName { get; set; } = null!;

    /// <summary>
    /// Identificação do fornecedor (supplier)
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Preço unitário do produto
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// O produto está descontinuado
    /// </summary>
    public bool IsDiscontinued { get; set; }

    /// <summary>
    /// Nome do pacote
    /// </summary>
    public string? PackageName { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;

}
