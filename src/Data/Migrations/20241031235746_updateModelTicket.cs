using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.src.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateModelTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "30e3f868-843b-42c7-a989-c48a6dbe08ac");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eba673d3-c0f2-45cb-b88c-11426bb22d88");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6c1cd9b3-5fe4-4283-8024-ff8409f70254", null, "Admin", "ADMIN" },
                    { "f5ad7ecf-ea5c-4797-a992-bbd43fe343b0", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c1cd9b3-5fe4-4283-8024-ff8409f70254");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f5ad7ecf-ea5c-4797-a992-bbd43fe343b0");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "30e3f868-843b-42c7-a989-c48a6dbe08ac", null, "User", "USER" },
                    { "eba673d3-c0f2-45cb-b88c-11426bb22d88", null, "Admin", "ADMIN" }
                });
        }
    }
}
