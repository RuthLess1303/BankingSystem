using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetBank.Db.Migrations
{
    /// <inheritdoc />
    public partial class currency_seeding_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "65dfd353-1654-4089-a902-2505b7c9294d", "AQAAAAIAAYagAAAAECoAAFVbVtIyeoMwLvnr8skFyfW0lRcoKMIa73UgmkL72ttZ53aP+vVRcEPqVPZ42Q==" });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Code", "Date", "Diff", "DiffFormatted", "Name", "Quantity", "Rate", "RateFormatted", "ValidFromDate" },
                values: new object[] { 300L, "GEL", new DateTimeOffset(new DateTime(2023, 4, 4, 14, 31, 46, 293, DateTimeKind.Unspecified).AddTicks(6740), new TimeSpan(0, 4, 0, 0, 0)), 0m, 0m, "ქართული ლარი", 1, 1m, 0m, new DateTimeOffset(new DateTime(2023, 4, 5, 14, 31, 46, 293, DateTimeKind.Unspecified).AddTicks(6790), new TimeSpan(0, 4, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: 300L);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b945f247-bb81-4ada-a710-761d5663c23d", "AQAAAAIAAYagAAAAEO9hL9plxrgBwM53n+jgdPsQiBDdpK7mnrkNlpoRQhd3WbHbxbgOg0x9LtfJOup9qQ==" });
        }
    }
}
