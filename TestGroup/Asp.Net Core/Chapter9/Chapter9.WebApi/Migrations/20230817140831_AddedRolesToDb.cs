using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Chapter9.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "674791cf-fba2-467f-911b-28a4a53eab87", "f89978dd-c277-4502-928d-6467a9743902", "Manager", "MANAGER" },
                    { "c8c4daa0-8e68-44ed-b69e-e7ad76a7d781", "7a04fc5c-8a1f-4b37-a419-04653755d6a3", "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "674791cf-fba2-467f-911b-28a4a53eab87");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8c4daa0-8e68-44ed-b69e-e7ad76a7d781");
        }
    }
}
