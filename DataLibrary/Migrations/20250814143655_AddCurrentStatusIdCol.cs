using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentStatusIdCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetTag",
                table: "EquipmentTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CurrentStatusId",
                table: "EquipmentTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CurrentStatus",
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
                    table.PrimaryKey("PK_CurrentStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTransactions_CurrentStatusId",
                table: "EquipmentTransactions",
                column: "CurrentStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTransactions_CurrentStatus_CurrentStatusId",
                table: "EquipmentTransactions",
                column: "CurrentStatusId",
                principalTable: "CurrentStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTransactions_CurrentStatus_CurrentStatusId",
                table: "EquipmentTransactions");

            migrationBuilder.DropTable(
                name: "CurrentStatus");

            migrationBuilder.DropIndex(
                name: "IX_EquipmentTransactions_CurrentStatusId",
                table: "EquipmentTransactions");

            migrationBuilder.DropColumn(
                name: "AssetTag",
                table: "EquipmentTransactions");

            migrationBuilder.DropColumn(
                name: "CurrentStatusId",
                table: "EquipmentTransactions");
        }
    }
}
