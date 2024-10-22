using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.src.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPurchaseOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f565475-c0bd-45fe-a9b4-2f870d166013");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a400edeb-0f70-4733-9cbe-1635b8e8fc98");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0003ce65-cefe-4747-b257-c8c633e246fd", null, "User", "USER" },
                    { "881bff06-7fe5-4ba2-b46b-e9b745686b33", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0003ce65-cefe-4747-b257-c8c633e246fd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "881bff06-7fe5-4ba2-b46b-e9b745686b33");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8f565475-c0bd-45fe-a9b4-2f870d166013", null, "Admin", "ADMIN" },
                    { "a400edeb-0f70-4733-9cbe-1635b8e8fc98", null, "User", "USER" }
                });
        }
    }
}
