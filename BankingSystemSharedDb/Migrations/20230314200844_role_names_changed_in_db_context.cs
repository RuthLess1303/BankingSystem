using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class role_names_changed_in_db_context : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "api-operator");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c0f9b3c5-edc7-4644-9037-db2f21a3d488", "AQAAAAIAAYagAAAAEGfmEqTYskqB7Eif9ZT1OBMO+dSqcZphOMyqRzspCK66h66r+w4kw0vCaukDf3w+rg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "ApiOperator");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3b8203c1-5ea3-4c93-b49b-ea9733fcfab0", "AQAAAAIAAYagAAAAEFSwTE3yItsB+S3Ia0q4pT+A/ZAOLAfqe1xuXroqMw5oMuwnCoLCv2iPfV+LSGuXCQ==" });
        }
    }
}
