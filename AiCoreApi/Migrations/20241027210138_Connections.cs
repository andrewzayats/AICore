using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class Connections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "token_cost");

            migrationBuilder.DropColumn(
                name: "model_type",
                table: "spent");

            migrationBuilder.AddColumn<string>(
                name: "model_name",
                table: "spent",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 10, 27, 21, 1, 37, 835, DateTimeKind.Utc).AddTicks(1015));

            migrationBuilder.Sql(
                @"delete from agents where name in ('BingSearchEnginePlugin', 'QuestionFromDialogPlugin', 'KernelMemoryPlugin')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "model_name",
                table: "spent");

            migrationBuilder.AddColumn<int>(
                name: "model_type",
                table: "spent",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "token_cost",
                columns: table => new
                {
                    token_cost_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    incoming = table.Column<decimal>(type: "numeric", nullable: false),
                    model_type = table.Column<int>(type: "integer", nullable: false),
                    outgoing = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_token_cost", x => x.token_cost_id);
                });

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 10, 13, 20, 21, 44, 427, DateTimeKind.Utc).AddTicks(422));

            migrationBuilder.InsertData(
                table: "token_cost",
                columns: new[] { "token_cost_id", "incoming", "model_type", "outgoing" },
                values: new object[,]
                {
                    { 1, 0m, 1, 0m },
                    { 2, 0.0000005m, 2, 0.0000015m },
                    { 3, 0.00001m, 3, 0.00003m }
                });
        }
    }
}
