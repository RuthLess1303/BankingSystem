using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class cvv_datatype_changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cvv",
                table: "Card",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldFixedLength: true,
                oldMaxLength: 3);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4c4738ea-0439-4d93-a896-5e805a2df5fd", "AQAAAAIAAYagAAAAEJy7TMhwmAYjsTbT9fPZ+d3XTBYeM5NHG9tdj3GoG0UqmjeOjdLztxRu2mWYjmlIqw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Cvv",
                table: "Card",
                type: "smallint",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(3)",
                oldFixedLength: true,
                oldMaxLength: 3);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "76d0755d-c7c5-4570-95fc-9665b5405d4b", "AQAAAAIAAYagAAAAEK672ot007fJjbzLKdfDME6KEGnjoIDvoFPGUiRI4j/73EPHOC6Zg/izN36jXrjYfg==" });
        }
    }
}
