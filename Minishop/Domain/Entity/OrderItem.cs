using System;
using System.Collections.Generic;

namespace Minishop.Domain.Entity;

public partial class OrderItem
{
    /// <summary>
    /// Chave primária(identificador) de cada registro da tabela
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificação do pedido (customer_order)
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Identificação do produto (product)
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Valor unitário
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Quantidade
    /// </summary>
    public int Quantity { get; set; }

    public virtual CustomerOrder Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
