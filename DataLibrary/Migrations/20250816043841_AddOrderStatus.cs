using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerProjectId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderConfirmStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActiveStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderConfirmStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerProjectId",
                table: "Orders",
                column: "CustomerProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CustomerProjects_CustomerProjectId",
                table: "Orders",
                column: "CustomerProjectId",
                principalTable: "CustomerProjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CustomerProjects_CustomerProjectId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderConfirmStatus");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerProjectId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerProjectId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Orders");
        }
    }
}
