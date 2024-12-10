using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCoreApi.Migrations;

/// <inheritdoc />
public partial class TaskToIngestForegnKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_task_ingestion_id",
            table: "task",
            column: "ingestion_id");

        migrationBuilder.AddForeignKey(
            name: "fk_task_ingestion_ingestion_id",
            table: "task",
            column: "ingestion_id",
            principalTable: "ingestion",
            principalColumn: "ingestion_id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_task_ingestion_ingestion_id",
            table: "task");

        migrationBuilder.DropIndex(
            name: "ix_task_ingestion_id",
            table: "task");
    }
}