using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class SchedulerAgentTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scheduler_agent_task",
                columns: table => new
                {
                    scheduler_agent_task_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scheduler_agent_task_state = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    valid_till = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    login_id = table.Column<int>(type: "integer", nullable: false),
                    request_accessor = table.Column<string>(type: "text", nullable: false),
                    scheduler_agent_task_guid = table.Column<string>(type: "text", nullable: false),
                    composite_agent_name = table.Column<string>(type: "text", nullable: false),
                    parameters = table.Column<string>(type: "text", nullable: false),
                    result = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scheduler_agent_task", x => x.scheduler_agent_task_id);
                });

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 10, 13, 20, 21, 44, 427, DateTimeKind.Utc).AddTicks(422));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scheduler_agent_task");

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 9, 23, 10, 5, 24, 509, DateTimeKind.Utc).AddTicks(3629));
        }
    }
}
