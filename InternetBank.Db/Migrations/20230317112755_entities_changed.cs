using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystemSharedDb.Migrations
{
    /// <inheritdoc />
    public partial class entities_changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "CardAccountConnection");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Account",
                newName: "Balance");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8da8cce1-65da-4fc0-88ac-1eb249f41daa", "AQAAAAIAAYagAAAAEBKs5fCZih+IqvvBMkUIL2VEhJgxaLpG8Lohur/HWOJkty9WZREyf4TOJLbpE/F4Zw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Account",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "CardAccountConnection",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "29627d51-565f-46f1-8320-c9e8b1b95d81", "AQAAAAIAAYagAAAAELiHwZvieAqP0TmvCl1f97/1NRKSCwrH6kmi58WyaZvlhdde9fWC+EwUGM20GvWEXw==" });
        }
    }
}
