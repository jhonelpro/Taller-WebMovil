using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.src.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingCartOptionsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02cf9924-37d7-4f63-a13a-9a8a3cf3446d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6333e3a7-4b66-4578-babf-888b1256b973");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "28b4ed10-70f1-4195-aa2e-45611cbcb7c0", null, "Admin", "ADMIN" },
                    { "45693865-f114-44fa-9c0e-2a7befbbf2a4", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "02cf9924-37d7-4f63-a13a-9a8a3cf3446d", null, "Admin", "ADMIN" },
                    { "6333e3a7-4b66-4578-babf-888b1256b973", null, "User", "USER" }
                });
        }
    }
}
