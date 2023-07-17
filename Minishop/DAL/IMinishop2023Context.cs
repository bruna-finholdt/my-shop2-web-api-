using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Minishop.Domain.Entity;

namespace Minishop.DAL
{
    public interface IMinishop2023Context
    {
        DbSet<Customer> Customers { get; set; }
        DbSet<CustomerOrder> CustomerOrders { get; set; }
        DbSet<OrderItem> OrderItems { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Supplier> Suppliers { get; set; }
    }
}
