﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minishop.DAL;

#nullable disable

namespace Minishop.DAL.Migrations
{
    [DbContext(typeof(Minishop2023Context))]
    [Migration("20230726150226_NewColumnProductTable")]
    partial class NewColumnProductTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Minishop.Domain.Entity.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Chave primária (identificador) de cada registro da tabela");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasMaxLength(11)
                        .IsUnicode(false)
                        .HasColumnType("varchar(11)")
                        .HasColumnName("CPF")
                        .HasComment("CPF do cliente");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasComment("E-mail do cliente");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Nome do cliente");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Sobrenome do cliente");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)")
                        .HasComment("Telefone do cliente");

                    b.HasKey("Id")
                        .HasName("PK_CUSTOMER");

                    b.HasIndex(new[] { "LastName", "FirstName" }, "IndexCustomerName");

                    b.HasIndex(new[] { "Email" }, "UQ__Customer__A9D10534A6D75C15")
                        .IsUnique();

                    b.HasIndex(new[] { "Cpf" }, "UQ__Customer__C1F897310A6CE8F6")
                        .IsUnique();

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("Minishop.Domain.Entity.CustomerOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Chave primária(identificador) de cada registro da tabela");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CustomerId")
                        .HasColumnType("int")
                        .HasComment("Identificação do cliente (customer)");

                    b.Property<DateTime>("OrderDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())")
                        .HasComment("Data do pedido");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(12, 2)")
                        .HasComment("Valor total do pedido");

                    b.HasKey("Id")
                        .HasName("PK_ORDER");

                    b.HasIndex(new[] { "CustomerId" }, "IndexOrderCustomerId");

                    b.HasIndex(new[] { "OrderDate" }, "IndexOrderOrderDate");

                    b.ToTable("CustomerOrder", (string)null);
                });

            modelBuilder.Entity("Minishop.Domain.Entity.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Chave primária(identificador) de cada registro da tabela");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasComment("Identificação do pedido (customer_order)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasComment("Identificação do produto (product)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasComment("Quantidade");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(12, 2)")
                        .HasComment("Valor unitário");

                    b.HasKey("Id")
                        .HasName("PK_ORDERITEM");

                    b.HasIndex(new[] { "OrderId" }, "IndexOrderItemOrderId");

                    b.HasIndex(new[] { "ProductId" }, "IndexOrderItemProductId");

                    b.ToTable("OrderItem", (string)null);
                });

            modelBuilder.Entity("Minishop.Domain.Entity.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Chave primária (identificador) de cada registro da tabela");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDiscontinued")
                        .HasColumnType("bit")
                        .HasComment("O produto está descontinuado");

                    b.Property<string>("PackageName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Nome do pacote");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Nome do produto");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int")
                        .HasComment("Identificação do fornecedor (supplier)");

                    b.Property<decimal?>("UnitPrice")
                        .HasColumnType("decimal(12, 2)")
                        .HasComment("Preço unitário do produto");

                    b.HasKey("Id")
                        .HasName("PK_PRODUCT");

                    b.HasIndex(new[] { "ProductName" }, "IndexProductName");

                    b.HasIndex(new[] { "SupplierId" }, "IndexProductSupplierId");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("Minishop.Domain.Entity.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Sequencia")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasComment("Chave primária (identificador) de cada registro da tabela");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Cidade do fornecedor");

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14)
                        .IsUnicode(false)
                        .HasColumnType("varchar(14)")
                        .HasColumnName("CNPJ")
                        .HasComment("CNPJ do fornecedor");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("Nome do fornecedor");

                    b.Property<string>("ContactName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("Nome do contato do fornecedor");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasComment("E-mail do fornecedor");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)")
                        .HasComment("Telefone do contato do fornecedor");

                    b.Property<string>("Uf")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)")
                        .HasColumnName("UF")
                        .HasComment("Estado do fornecedor");

                    b.HasKey("Id")
                        .HasName("PK_SUPPLIER");

                    b.HasIndex(new[] { "CompanyName" }, "IndexSupplierName");

                    b.HasIndex(new[] { "Email" }, "UQ__Supplier__A9D10534AD741EC7")
                        .IsUnique();

                    b.HasIndex(new[] { "Cnpj" }, "UQ__Supplier__AA57D6B4F578A1DB")
                        .IsUnique();

                    b.ToTable("Supplier", (string)null);
                });

            modelBuilder.Entity("Minishop.Domain.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.CustomerOrder", b =>
                {
                    b.HasOne("Minishop.Domain.Entity.Customer", "Customer")
                        .WithMany("CustomerOrders")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_ORDER_REFERENCE_CUSTOMER");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.OrderItem", b =>
                {
                    b.HasOne("Minishop.Domain.Entity.CustomerOrder", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .IsRequired()
                        .HasConstraintName("FK_ORDERITE_REFERENCE_ORDER");

                    b.HasOne("Minishop.Domain.Entity.Product", "Product")
                        .WithMany("OrderItems")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_ORDERITE_REFERENCE_PRODUCT");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.Product", b =>
                {
                    b.HasOne("Minishop.Domain.Entity.Supplier", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("SupplierId")
                        .IsRequired()
                        .HasConstraintName("FK_PRODUCT_REFERENCE_SUPPLIER");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.ProductImage", b =>
                {
                    b.HasOne("Minishop.Domain.Entity.Product", "Produto")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Produto");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.User", b =>
                {
                    b.HasOne("Minishop.Domain.Entity.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.Customer", b =>
                {
                    b.Navigation("CustomerOrders");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.CustomerOrder", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.Product", b =>
                {
                    b.Navigation("OrderItems");

                    b.Navigation("ProductImages");
                });

            modelBuilder.Entity("Minishop.Domain.Entity.Supplier", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
