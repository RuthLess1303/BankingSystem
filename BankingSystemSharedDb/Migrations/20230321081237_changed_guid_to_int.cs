using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class changed_guid_to_int : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "6a143114-c9b2-4105-9d88-cdea08fd6451", "AQAAAAIAAYagAAAAEOWMWxdfuMTlw13mhOVytGohIQz56h93shnpizfGNRJMrP84ImASFAtKJqKiN3+z4A==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4c4738ea-0439-4d93-a896-5e805a2df5fd", "AQAAAAIAAYagAAAAEJy7TMhwmAYjsTbT9fPZ+d3XTBYeM5NHG9tdj3GoG0UqmjeOjdLztxRu2mWYjmlIqw==" });
        }
    }
}
