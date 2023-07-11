using System;
using System.Collections.Generic;

namespace Minishop.Domain.Entity;

public partial class CustomerOrder
{
    /// <summary>
    /// Chave primária(identificador) de cada registro da tabela
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Data do pedido
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Identificação do cliente (customer)
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Valor total do pedido
    /// </summary>
    public decimal TotalAmount { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
