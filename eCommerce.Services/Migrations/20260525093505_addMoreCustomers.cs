using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eCommerce.Services.Migrations
{
    /// <inheritdoc />
    public partial class addMoreCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastLoginAt", "LastName", "PasswordHash", "PasswordSalt", "PhoneNumber", "ProfileImageBase64", "Username" },
                values: new object[,]
                {
                    { 6, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "customer3@gmail.com", "Frank", true, null, "Customer", "Ov4LxpWKXXV9dwMYvBgqODdzIt0=", "KtWF6g7SemBqs4nVWV4Ziw==", null, null, "customer3" },
                    { 7, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "customer4@gmail.com", "Grace", true, null, "Customer", "Ov4LxpWKXXV9dwMYvBgqODdzIt0=", "KtWF6g7SemBqs4nVWV4Ziw==", null, null, "customer4" },
                    { 8, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "customer5@gmail.com", "Hank", true, null, "Customer", "Ov4LxpWKXXV9dwMYvBgqODdzIt0=", "KtWF6g7SemBqs4nVWV4Ziw==", null, null, "customer5" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "DateAssigned", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 6, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), 2, 6 },
                    { 7, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), 2, 7 },
                    { 8, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), 2, 8 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
