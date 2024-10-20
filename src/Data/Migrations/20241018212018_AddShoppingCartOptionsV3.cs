using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.src.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingCartOptionsV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "28b4ed10-70f1-4195-aa2e-45611cbcb7c0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "45693865-f114-44fa-9c0e-2a7befbbf2a4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "396b0897-660d-4b48-9c66-1883a7a4cfa2", null, "User", "USER" },
                    { "5e30a425-383b-48f7-9309-e1f98b3310f6", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "396b0897-660d-4b48-9c66-1883a7a4cfa2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e30a425-383b-48f7-9309-e1f98b3310f6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "28b4ed10-70f1-4195-aa2e-45611cbcb7c0", null, "Admin", "ADMIN" },
                    { "45693865-f114-44fa-9c0e-2a7befbbf2a4", null, "User", "USER" }
                });
        }
    }
}
