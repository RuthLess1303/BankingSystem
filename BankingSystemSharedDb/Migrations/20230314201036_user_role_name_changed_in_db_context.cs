using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class user_role_name_changed_in_db_context : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "api-user");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a7971da5-f33f-487d-a9f2-b0d2510aa00a", "AQAAAAIAAYagAAAAEBWM0/JQIsotRIJOaqrqojfXmpGTh9LMA3OLXuD0+rDDy9byBssl3AnBW4czBaOwSQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "ApiUser");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c0f9b3c5-edc7-4644-9037-db2f21a3d488", "AQAAAAIAAYagAAAAEGfmEqTYskqB7Eif9ZT1OBMO+dSqcZphOMyqRzspCK66h66r+w4kw0vCaukDf3w+rg==" });
        }
    }
}
