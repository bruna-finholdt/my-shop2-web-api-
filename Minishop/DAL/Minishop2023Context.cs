using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Minishop.DAL.Repositories;
using Minishop.Domain.Entity;

namespace Minishop.DAL;

public partial class Minishop2023Context : DbContext, ICustomersDbContext
{

    public Minishop2023Context(DbContextOptions<Minishop2023Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerOrder> CustomerOrders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    EntityEntry ICustomersDbContext.Entry(object entity) => Entry(entity);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CUSTOMER");

            entity.ToTable("Customer");

            entity.HasIndex(e => new { e.LastName, e.FirstName }, "IndexCustomerName");

            entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534A6D75C15").IsUnique();

            entity.HasIndex(e => e.Cpf, "UQ__Customer__C1F897310A6CE8F6").IsUnique();

            entity.Property(e => e.Id).HasComment("Chave primária (identificador) de cada registro da tabela");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasComment("CPF do cliente")
                .HasColumnName("CPF");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasComment("E-mail do cliente");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasComment("Nome do cliente");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasComment("Sobrenome do cliente");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComment("Telefone do cliente");
        });

        modelBuilder.Entity<CustomerOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ORDER");

            entity.ToTable("CustomerOrder");

            entity.HasIndex(e => e.CustomerId, "IndexOrderCustomerId");

            entity.HasIndex(e => e.OrderDate, "IndexOrderOrderDate");

            entity.Property(e => e.Id).HasComment("Chave primária(identificador) de cada registro da tabela");
            entity.Property(e => e.CustomerId).HasComment("Identificação do cliente (customer)");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Data do pedido")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalAmount)
                .HasComment("Valor total do pedido")
                .HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDER_REFERENCE_CUSTOMER");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ORDERITEM");

            entity.ToTable("OrderItem");

            entity.HasIndex(e => e.OrderId, "IndexOrderItemOrderId");

            entity.HasIndex(e => e.ProductId, "IndexOrderItemProductId");

            entity.Property(e => e.Id).HasComment("Chave primária(identificador) de cada registro da tabela");
            entity.Property(e => e.OrderId).HasComment("Identificação do pedido (customer_order)");
            entity.Property(e => e.ProductId).HasComment("Identificação do produto (product)");
            entity.Property(e => e.Quantity).HasComment("Quantidade");
            entity.Property(e => e.UnitPrice)
                .HasComment("Valor unitário")
                .HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDERITE_REFERENCE_ORDER");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ORDERITE_REFERENCE_PRODUCT");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PRODUCT");

            entity.ToTable("Product");

            entity.HasIndex(e => e.ProductName, "IndexProductName");

            entity.HasIndex(e => e.SupplierId, "IndexProductSupplierId");

            entity.Property(e => e.Id).HasComment("Chave primária (identificador) de cada registro da tabela");
            entity.Property(e => e.IsDiscontinued).HasComment("O produto está descontinuado");
            entity.Property(e => e.PackageName)
                .HasMaxLength(100)
                .HasComment("Nome do pacote");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasComment("Nome do produto");
            entity.Property(e => e.SupplierId).HasComment("Identificação do fornecedor (supplier)");
            entity.Property(e => e.UnitPrice)
                .HasComment("Preço unitário do produto")
                .HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRODUCT_REFERENCE_SUPPLIER");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SUPPLIER");

            entity.ToTable("Supplier");

            entity.HasIndex(e => e.CompanyName, "IndexSupplierName");

            entity.HasIndex(e => e.Email, "UQ__Supplier__A9D10534AD741EC7").IsUnique();

            entity.HasIndex(e => e.Cnpj, "UQ__Supplier__AA57D6B4F578A1DB").IsUnique();

            entity.Property(e => e.Id).HasComment("Chave primária (identificador) de cada registro da tabela");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasComment("Cidade do fornecedor");
            entity.Property(e => e.Cnpj)
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasComment("CNPJ do fornecedor")
                .HasColumnName("CNPJ");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasComment("Nome do fornecedor");
            entity.Property(e => e.ContactName)
                .HasMaxLength(50)
                .HasComment("Nome do contato do fornecedor");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasComment("E-mail do fornecedor");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComment("Telefone do contato do fornecedor");
            entity.Property(e => e.Uf)
                .HasMaxLength(2)
                .HasComment("Estado do fornecedor")
                .HasColumnName("UF");
        });

    }
}
