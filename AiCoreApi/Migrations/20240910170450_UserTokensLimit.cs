using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class UserTokensLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "tokens_limit",
                table: "login",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                columns: new[] { "created", "tokens_limit" },
                values: new object[] { new DateTime(2024, 9, 10, 17, 4, 50, 488, DateTimeKind.Utc).AddTicks(718), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tokens_limit",
                table: "login");

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 7, 23, 13, 58, 38, 660, DateTimeKind.Utc).AddTicks(3128));
        }
    }
}
