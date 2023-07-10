using System;
using System.Collections.Generic;

namespace Minishop.Domain.Entity;

public partial class Customer
{
    /// <summary>
    /// Chave primária (identificador) de cada registro da tabela
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do cliente
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Sobrenome do cliente
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// CPF do cliente
    /// </summary>
    public string Cpf { get; set; } = null!;

    /// <summary>
    /// E-mail do cliente
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Telefone do cliente
    /// </summary>
    public string? Phone { get; set; }

    public virtual ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();
}
