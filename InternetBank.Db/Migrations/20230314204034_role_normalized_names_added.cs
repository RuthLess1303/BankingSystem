using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class role_normalized_names_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "NormalizedName",
                value: "API-USER");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "NormalizedName",
                value: "API-OPERATOR");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "29627d51-565f-46f1-8320-c9e8b1b95d81", "AQAAAAIAAYagAAAAELiHwZvieAqP0TmvCl1f97/1NRKSCwrH6kmi58WyaZvlhdde9fWC+EwUGM20GvWEXw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a7971da5-f33f-487d-a9f2-b0d2510aa00a", "AQAAAAIAAYagAAAAEBWM0/JQIsotRIJOaqrqojfXmpGTh9LMA3OLXuD0+rDDy9byBssl3AnBW4czBaOwSQ==" });
        }
    }
}
