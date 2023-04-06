using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InternetBank.Db.Migrations
{
    /// <inheritdoc />
    public partial class login_logger_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.CreateTable(
                name: "LoginLogger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginLogger", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "650c6b60-0732-4369-9c21-0305fbcac269", "AQAAAAIAAYagAAAAECgSURX3RiFbgeNF+lbHdI6pnD46CwDu4YCupgtVnsxkIIimdvVw2f5v+ZSnEaDQNw==" });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Code", "Date", "Diff", "DiffFormatted", "Name", "Quantity", "Rate", "RateFormatted", "RatePerQuantity", "ValidFromDate" },
                values: new object[] { 300L, "GEL", new DateTimeOffset(new DateTime(2023, 4, 6, 15, 38, 26, 6, DateTimeKind.Unspecified).AddTicks(8760), new TimeSpan(0, 4, 0, 0, 0)), 0m, 0m, "ქართული ლარი", 1, 1m, 0m, 0m, new DateTimeOffset(new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginLogger");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 300L);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7f38605e-5665-4f20-838d-ebb246026a27", "AQAAAAIAAYagAAAAEEhsY2SztbyUzkNJbMjL9stX+HTece1Jy1UpLEqQWSvH0HOG8MmLqJIStVXeEIvyqg==" });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Code", "Date", "Diff", "DiffFormatted", "Name", "Quantity", "Rate", "RateFormatted", "RatePerQuantity", "ValidFromDate" },
                values: new object[,]
                {
                    { 1L, "GEL", new DateTimeOffset(new DateTime(2023, 4, 4, 21, 2, 58, 584, DateTimeKind.Unspecified).AddTicks(4891), new TimeSpan(0, 4, 0, 0, 0)), 0m, 0m, "ქართული ლარი", 1, 1m, 0m, 0m, new DateTimeOffset(new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2L, "USD", new DateTimeOffset(new DateTime(2023, 4, 4, 21, 2, 58, 584, DateTimeKind.Unspecified).AddTicks(4920), new TimeSpan(0, 4, 0, 0, 0)), 0m, 0m, "დოლარი", 1, 1m, 0m, 0m, new DateTimeOffset(new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3L, "EUR", new DateTimeOffset(new DateTime(2023, 4, 4, 21, 2, 58, 584, DateTimeKind.Unspecified).AddTicks(4922), new TimeSpan(0, 4, 0, 0, 0)), 0m, 0m, "ეურო", 1, 1m, 0m, 0m, new DateTimeOffset(new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new TimeSpan(0, 0, 0, 0, 0)) }
                });
        }
    }
}
