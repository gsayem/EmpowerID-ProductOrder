using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpowerID.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    category_id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    category_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    order_id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    order_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    customer_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.order_id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    product_id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    category_id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    date_added = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_category_id",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductOrders",
                columns: table => new
                {
                    product_order_id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    product_id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    order_id = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOrders", x => x.product_order_id);
                    table.ForeignKey(
                        name: "FK_ProductOrders_Orders_order_id",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductOrders_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "CategoryName_Unique_NonClustered_Index",
                table: "Categories",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "OrderDate_NonClustered_Index",
                table: "Orders",
                column: "order_date");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_order_id",
                table: "ProductOrders",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_product_id",
                table: "ProductOrders",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_category_id",
                table: "Products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ProductDateAddded_Non_Clustered_Index",
                table: "Products",
                column: "date_added");

            migrationBuilder.CreateIndex(
                name: "ProductName_Unique_NonClustered_Index",
                table: "Products",
                column: "product_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ProductPrice_Non_Clustered_Index",
                table: "Products",
                column: "price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductOrders");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
