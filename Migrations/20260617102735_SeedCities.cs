using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zariya.Migrations
{
    /// <inheritdoc />
    public partial class SeedCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed major Pakistani cities
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "city_id", "city_name", "province", "is_active", "created_at" },
                values: new object[,]
                {
                    { 1, "Lahore", "Punjab", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Karachi", "Sindh", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Islamabad", "Islamabad Capital Territory", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Rawalpindi", "Punjab", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "Faisalabad", "Punjab", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "Multan", "Punjab", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "Peshawar", "Khyber Pakhtunkhwa", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "Quetta", "Balochistan", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "Gujranwala", "Punjab", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "Sialkot", "Punjab", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "Hyderabad", "Sindh", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "Abbottabad", "Khyber Pakhtunkhwa", true, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "city_id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
        }
    }
}
