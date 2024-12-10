using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class VersionToAgents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "version",
                table: "agents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 9, 23, 10, 5, 24, 509, DateTimeKind.Utc).AddTicks(3629));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "version",
                table: "agents");

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 9, 10, 17, 4, 50, 488, DateTimeKind.Utc).AddTicks(718));
        }
    }
}
