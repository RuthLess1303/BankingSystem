using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class role_names_changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "ApiUser");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "user");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "operator");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "44e3e7dc-79c5-4f71-bf95-16b27d38e364", "AQAAAAIAAYagAAAAECa2TuFBt3nhV7ba4YDW1J/VOv+78CphxiRz4+EFUVtF45APHPLoa/nzqvmQ8Jkosw==" });
        }
    }
}
