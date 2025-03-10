using System;
using System.Collections.Generic;
using AiCoreApi.Models.DbModels;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class DebugLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "debug_log",
                columns: table => new
                {
                    debug_log_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    result = table.Column<string>(type: "text", nullable: false),
                    debug_messages = table.Column<List<DebugMessage>>(type: "jsonb", nullable: true),
                    spent_tokens = table.Column<Dictionary<string, TokensSpent>>(type: "jsonb", nullable: true),
                    files = table.Column<List<string>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_debug_log", x => x.debug_log_id);
                });

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2025, 3, 7, 11, 37, 33, 611, DateTimeKind.Utc).AddTicks(4272));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "debug_log");

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 12, 10, 14, 7, 46, 705, DateTimeKind.Utc).AddTicks(9951));
        }
    }
}
