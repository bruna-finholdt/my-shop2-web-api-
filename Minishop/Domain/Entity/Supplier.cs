using System;
using System.Collections.Generic;

namespace Minishop.Domain.Entity;

public partial class Supplier
{
    /// <summary>
    /// Chave primária (identificador) de cada registro da tabela
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do fornecedor
    /// </summary>
    public string CompanyName { get; set; } = null!;

    /// <summary>
    /// CNPJ do fornecedor
    /// </summary>
    public string Cnpj { get; set; } = null!;

    /// <summary>
    /// E-mail do fornecedor
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Cidade do fornecedor
    /// </summary>
    public string City { get; set; } = null!;

    /// <summary>
    /// Estado do fornecedor
    /// </summary>
    public string Uf { get; set; } = null!;

    /// <summary>
    /// Nome do contato do fornecedor
    /// </summary>
    public string? ContactName { get; set; }

    /// <summary>
    /// Telefone do contato do fornecedor
    /// </summary>
    public string? Phone { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
