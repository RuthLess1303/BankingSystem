using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetBank.Db.Migrations
{
    /// <inheritdoc />
    public partial class logger_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThrowTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logger", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b945f247-bb81-4ada-a710-761d5663c23d", "AQAAAAIAAYagAAAAEO9hL9plxrgBwM53n+jgdPsQiBDdpK7mnrkNlpoRQhd3WbHbxbgOg0x9LtfJOup9qQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logger");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0fb3f79c-2ecc-4a04-a6e8-63f05f7f804e", "AQAAAAIAAYagAAAAEHlICcMQBRbIkHLl1uT6cuVevyhfLpsMd7f8ns8eh0xQImHz8N3JKKU/AXRCxQFLxw==" });
        }
    }
}
