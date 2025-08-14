using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLibrary.Migrations
{
    /// <inheritdoc />
    public partial class RelationEquipmentSubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultRentalRateDaily",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultRentalRateMonthly",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultRentalRateOneTime",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultRentalRateWeekly",
                table: "Equipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SubcategoryId",
                table: "Equipments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_SubcategoryId",
                table: "Equipments",
                column: "SubcategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Subcategories_SubcategoryId",
                table: "Equipments",
                column: "SubcategoryId",
                principalTable: "Subcategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Subcategories_SubcategoryId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_SubcategoryId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "DefaultRentalRateDaily",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "DefaultRentalRateMonthly",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "DefaultRentalRateOneTime",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "DefaultRentalRateWeekly",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "SubcategoryId",
                table: "Equipments");
        }
    }
}
