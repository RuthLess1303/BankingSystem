using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class entities_properties_changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Pin",
                table: "Card",
                type: "nchar(4)",
                fixedLength: true,
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldFixedLength: true,
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<short>(
                name: "Cvv",
                table: "Card",
                type: "smallint",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldFixedLength: true,
                oldMaxLength: 3);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "76d0755d-c7c5-4570-95fc-9665b5405d4b", "AQAAAAIAAYagAAAAEK672ot007fJjbzLKdfDME6KEGnjoIDvoFPGUiRI4j/73EPHOC6Zg/izN36jXrjYfg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Pin",
                table: "Card",
                type: "int",
                fixedLength: true,
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(4)",
                oldFixedLength: true,
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<int>(
                name: "Cvv",
                table: "Card",
                type: "int",
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
                values: new object[] { "8da8cce1-65da-4fc0-88ac-1eb249f41daa", "AQAAAAIAAYagAAAAEBKs5fCZih+IqvvBMkUIL2VEhJgxaLpG8Lohur/HWOJkty9WZREyf4TOJLbpE/F4Zw==" });
        }
    }
}
