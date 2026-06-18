using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zariya.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Donation_Types",
                keyColumn: "type_id",
                keyValue: 1,
                columns: new[] { "description", "type_name" },
                values: new object[] { "Standard whole blood donation", "Whole Blood" });

            migrationBuilder.UpdateData(
                table: "Donation_Types",
                keyColumn: "type_id",
                keyValue: 2,
                columns: new[] { "description", "type_name" },
                values: new object[] { "Platelet donation", "Platelets" });

            migrationBuilder.UpdateData(
                table: "Donation_Types",
                keyColumn: "type_id",
                keyValue: 3,
                columns: new[] { "description", "type_name" },
                values: new object[] { "Plasma donation", "Plasma" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Donation_Types",
                keyColumn: "type_id",
                keyValue: 1,
                columns: new[] { "description", "type_name" },
                values: new object[] { "Monetary donation", "Money" });

            migrationBuilder.UpdateData(
                table: "Donation_Types",
                keyColumn: "type_id",
                keyValue: 2,
                columns: new[] { "description", "type_name" },
                values: new object[] { "Clothing donation", "Clothes" });

            migrationBuilder.UpdateData(
                table: "Donation_Types",
                keyColumn: "type_id",
                keyValue: 3,
                columns: new[] { "description", "type_name" },
                values: new object[] { "Food donation", "Food" });
        }
    }
}
