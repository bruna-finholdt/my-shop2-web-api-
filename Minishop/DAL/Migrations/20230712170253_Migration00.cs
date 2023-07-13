using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minishop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Migration00 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Chave primária (identificador) de cada registro da tabela")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Nome do cliente"),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Sobrenome do cliente"),
                    CPF = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: false, comment: "CPF do cliente"),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false, comment: "E-mail do cliente"),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true, comment: "Telefone do cliente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Chave primária (identificador) de cada registro da tabela")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Nome do fornecedor"),
                    CNPJ = table.Column<string>(type: "varchar(14)", unicode: false, maxLength: 14, nullable: false, comment: "CNPJ do fornecedor"),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false, comment: "E-mail do fornecedor"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Cidade do fornecedor"),
                    UF = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, comment: "Estado do fornecedor"),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Nome do contato do fornecedor"),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true, comment: "Telefone do contato do fornecedor")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUPPLIER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Chave primária(identificador) de cada registro da tabela")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "Data do pedido"),
                    CustomerId = table.Column<int>(type: "int", nullable: false, comment: "Identificação do cliente (customer)"),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false, comment: "Valor total do pedido")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ORDER_REFERENCE_CUSTOMER",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Chave primária (identificador) de cada registro da tabela")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Nome do produto"),
                    SupplierId = table.Column<int>(type: "int", nullable: false, comment: "Identificação do fornecedor (supplier)"),
                    UnitPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: true, comment: "Preço unitário do produto"),
                    IsDiscontinued = table.Column<bool>(type: "bit", nullable: false, comment: "O produto está descontinuado"),
                    PackageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Nome do pacote")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRODUCT_REFERENCE_SUPPLIER",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Chave primária(identificador) de cada registro da tabela")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false, comment: "Identificação do pedido (customer_order)"),
                    ProductId = table.Column<int>(type: "int", nullable: false, comment: "Identificação do produto (product)"),
                    UnitPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false, comment: "Valor unitário"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Quantidade")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDERITEM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ORDERITE_REFERENCE_ORDER",
                        column: x => x.OrderId,
                        principalTable: "CustomerOrder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ORDERITE_REFERENCE_PRODUCT",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IndexCustomerName",
                table: "Customer",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__A9D10534A6D75C15",
                table: "Customer",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__C1F897310A6CE8F6",
                table: "Customer",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IndexOrderCustomerId",
                table: "CustomerOrder",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IndexOrderOrderDate",
                table: "CustomerOrder",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IndexOrderItemOrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IndexOrderItemProductId",
                table: "OrderItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IndexProductName",
                table: "Product",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IndexProductSupplierId",
                table: "Product",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IndexSupplierName",
                table: "Supplier",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "UQ__Supplier__A9D10534AD741EC7",
                table: "Supplier",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Supplier__AA57D6B4F578A1DB",
                table: "Supplier",
                column: "CNPJ",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "CustomerOrder");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
