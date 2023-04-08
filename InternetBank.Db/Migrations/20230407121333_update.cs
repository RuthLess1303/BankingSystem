using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetBank.Db.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "f084f584-17b3-40ae-af0c-f9cbe9be5f56", "AQAAAAIAAYagAAAAEJcsZPrCGyK/Fsnp0jPD6/PXZLRR16LWQXYXmHwaT0jL8s6+BYZnpanIss0mhNfxXA==" });

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Date",
                value: new DateTimeOffset(new DateTime(2023, 4, 7, 16, 13, 33, 684, DateTimeKind.Unspecified).AddTicks(2560), new TimeSpan(0, 4, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "bdb4f8e4-cf91-41f2-9822-6d7b47b51b85", "AQAAAAIAAYagAAAAEA1sDPWs8Aq0d7n8QrI43mCl25a4YKkceb5FVMA15+c/IjIquPH32xZaIhxEWc7bRg==" });

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Date",
                value: new DateTimeOffset(new DateTime(2023, 4, 6, 20, 59, 14, 973, DateTimeKind.Unspecified).AddTicks(907), new TimeSpan(0, 4, 0, 0, 0)));
        }
    }
}
