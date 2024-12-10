using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class LlmAgents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agents",
                columns: table => new
                {
                    agent_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    llm_type = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_agents", x => x.agent_id);
                });

            migrationBuilder.CreateTable(
                name: "tags_x_agents",
                columns: table => new
                {
                    agents_agent_id = table.Column<int>(type: "integer", nullable: false),
                    tags_tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags_x_agents", x => new { x.agents_agent_id, x.tags_tag_id });
                    table.ForeignKey(
                        name: "fk_tags_x_agents_agents_agents_agent_id",
                        column: x => x.agents_agent_id,
                        principalTable: "agents",
                        principalColumn: "agent_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tags_x_agents_tags_tags_tag_id",
                        column: x => x.tags_tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 7, 4, 13, 18, 26, 293, DateTimeKind.Utc).AddTicks(1609));

            migrationBuilder.CreateIndex(
                name: "ix_tags_x_agents_tags_tag_id",
                table: "tags_x_agents",
                column: "tags_tag_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tags_x_agents");

            migrationBuilder.DropTable(
                name: "agents");

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 6, 26, 10, 26, 23, 736, DateTimeKind.Utc).AddTicks(1808));
        }
    }
}
