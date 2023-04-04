using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetBank.Db.Migrations
{
    /// <inheritdoc />
    public partial class changed_mapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Currency_Code",
                table: "Currency");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currency",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0fb3f79c-2ecc-4a04-a6e8-63f05f7f804e", "AQAAAAIAAYagAAAAEHlICcMQBRbIkHLl1uT6cuVevyhfLpsMd7f8ns8eh0xQImHz8N3JKKU/AXRCxQFLxw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currency",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Currency_Code",
                table: "Currency",
                column: "Code");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4cc147e9-208f-43bd-898e-fff3a991a48e", "AQAAAAIAAYagAAAAEHnie3VU9pFO/D0aT/oOaGndyqs7+bJc5N4hHK8AA3lWRWT01gyig+pG+wGyypiq/g==" });
        }
    }
}
