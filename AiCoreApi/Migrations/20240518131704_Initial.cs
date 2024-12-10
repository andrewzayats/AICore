using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client_sso",
                columns: table => new
                {
                    client_sso_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    login_type = table.Column<int>(type: "integer", nullable: false),
                    settings = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_sso", x => x.client_sso_id);
                });

            migrationBuilder.CreateTable(
                name: "document_metadata",
                columns: table => new
                {
                    document_id = table.Column<string>(type: "text", nullable: false),
                    ingestion_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    created_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_metadata_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_metadata", x => x.document_id);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "ingestion",
                columns: table => new
                {
                    ingestion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_sync = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ingestion", x => x.ingestion_id);
                });

            migrationBuilder.CreateTable(
                name: "login",
                columns: table => new
                {
                    login_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    login_type = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login", x => x.login_id);
                });

            migrationBuilder.CreateTable(
                name: "login_history",
                columns: table => new
                {
                    login_history_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_id = table.Column<int>(type: "integer", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    code_challenge = table.Column<string>(type: "text", nullable: true),
                    refresh_token = table.Column<string>(type: "text", nullable: true),
                    is_offline = table.Column<bool>(type: "boolean", nullable: false),
                    valid_until_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_login_history", x => x.login_history_id);
                });

            migrationBuilder.CreateTable(
                name: "rbac_group_sync",
                columns: table => new
                {
                    rbac_group_sync_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rbac_group_name = table.Column<string>(type: "text", nullable: false),
                    ai_core_group_name = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rbac_group_sync", x => x.rbac_group_sync_id);
                });

            migrationBuilder.CreateTable(
                name: "rbac_role_sync",
                columns: table => new
                {
                    rbac_role_sync_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rbac_role_name = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rbac_role_sync", x => x.rbac_role_sync_id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    settings_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entity_id = table.Column<int>(type: "integer", nullable: true),
                    settings_type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.settings_id);
                });

            migrationBuilder.CreateTable(
                name: "spent",
                columns: table => new
                {
                    spent_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_id = table.Column<int>(type: "integer", nullable: false),
                    model_type = table.Column<int>(type: "integer", nullable: false),
                    tokens_outgoing = table.Column<int>(type: "integer", nullable: false),
                    tokens_incoming = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spent", x => x.spent_id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    color = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "task",
                columns: table => new
                {
                    task_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ingestion_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    context = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_task", x => x.task_id);
                });

            migrationBuilder.CreateTable(
                name: "token_cost",
                columns: table => new
                {
                    token_cost_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    model_type = table.Column<int>(type: "integer", nullable: false),
                    outgoing = table.Column<decimal>(type: "numeric", nullable: false),
                    incoming = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_token_cost", x => x.token_cost_id);
                });

            migrationBuilder.CreateTable(
                name: "client_sso_x_groups",
                columns: table => new
                {
                    client_sso_id = table.Column<int>(type: "integer", nullable: false),
                    groups_group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_sso_x_groups", x => new { x.client_sso_id, x.groups_group_id });
                    table.ForeignKey(
                        name: "fk_client_sso_x_groups_client_sso_client_sso_id",
                        column: x => x.client_sso_id,
                        principalTable: "client_sso",
                        principalColumn: "client_sso_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_client_sso_x_groups_groups_groups_group_id",
                        column: x => x.groups_group_id,
                        principalTable: "groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logins_x_groups",
                columns: table => new
                {
                    groups_group_id = table.Column<int>(type: "integer", nullable: false),
                    logins_login_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logins_x_groups", x => new { x.groups_group_id, x.logins_login_id });
                    table.ForeignKey(
                        name: "fk_logins_x_groups_groups_groups_group_id",
                        column: x => x.groups_group_id,
                        principalTable: "groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_logins_x_groups_login_logins_login_id",
                        column: x => x.logins_login_id,
                        principalTable: "login",
                        principalColumn: "login_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags_x_groups",
                columns: table => new
                {
                    groups_group_id = table.Column<int>(type: "integer", nullable: false),
                    tags_tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags_x_groups", x => new { x.groups_group_id, x.tags_tag_id });
                    table.ForeignKey(
                        name: "fk_tags_x_groups_groups_groups_group_id",
                        column: x => x.groups_group_id,
                        principalTable: "groups",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tags_x_groups_tags_tags_tag_id",
                        column: x => x.tags_tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags_x_ingestions",
                columns: table => new
                {
                    ingestions_ingestion_id = table.Column<int>(type: "integer", nullable: false),
                    tags_tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags_x_ingestions", x => new { x.ingestions_ingestion_id, x.tags_tag_id });
                    table.ForeignKey(
                        name: "fk_tags_x_ingestions_ingestion_ingestions_ingestion_id",
                        column: x => x.ingestions_ingestion_id,
                        principalTable: "ingestion",
                        principalColumn: "ingestion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tags_x_ingestions_tags_tags_tag_id",
                        column: x => x.tags_tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags_x_logins",
                columns: table => new
                {
                    logins_login_id = table.Column<int>(type: "integer", nullable: false),
                    tags_tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags_x_logins", x => new { x.logins_login_id, x.tags_tag_id });
                    table.ForeignKey(
                        name: "fk_tags_x_logins_login_logins_login_id",
                        column: x => x.logins_login_id,
                        principalTable: "login",
                        principalColumn: "login_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tags_x_logins_tags_tags_tag_id",
                        column: x => x.tags_tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tags_x_rbac_role_sync",
                columns: table => new
                {
                    rbac_role_syncs_rbac_role_sync_id = table.Column<int>(type: "integer", nullable: false),
                    tags_tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags_x_rbac_role_sync", x => new { x.rbac_role_syncs_rbac_role_sync_id, x.tags_tag_id });
                    table.ForeignKey(
                        name: "fk_tags_x_rbac_role_sync_rbac_role_sync_rbac_role_syncs_rbac_r~",
                        column: x => x.rbac_role_syncs_rbac_role_sync_id,
                        principalTable: "rbac_role_sync",
                        principalColumn: "rbac_role_sync_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tags_x_rbac_role_sync_tags_tags_tag_id",
                        column: x => x.tags_tag_id,
                        principalTable: "tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "login",
                columns: new[] { "login_id", "created", "created_by", "email", "full_name", "is_enabled", "login", "login_type", "password_hash", "role" },
                values: new object[] { 1, new DateTime(2024, 5, 18, 13, 17, 3, 559, DateTimeKind.Utc).AddTicks(5246), "system", "admin@viacode.com", "Admin", true, "admin@viacode.com", 1, "wh+Wm18D0z1D4E+PE252gg==", 2 });

            migrationBuilder.InsertData(
                table: "token_cost",
                columns: new[] { "token_cost_id", "incoming", "model_type", "outgoing" },
                values: new object[,]
                {
                    { 1, 0m, 1, 0m },
                    { 2, 0.0000005m, 2, 0.0000015m },
                    { 3, 0.00001m, 3, 0.00003m }
                });

            migrationBuilder.CreateIndex(
                name: "ix_client_sso_x_groups_groups_group_id",
                table: "client_sso_x_groups",
                column: "groups_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_logins_x_groups_logins_login_id",
                table: "logins_x_groups",
                column: "logins_login_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_x_groups_tags_tag_id",
                table: "tags_x_groups",
                column: "tags_tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_x_ingestions_tags_tag_id",
                table: "tags_x_ingestions",
                column: "tags_tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_x_logins_tags_tag_id",
                table: "tags_x_logins",
                column: "tags_tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_x_rbac_role_sync_tags_tag_id",
                table: "tags_x_rbac_role_sync",
                column: "tags_tag_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "client_sso_x_groups");

            migrationBuilder.DropTable(
                name: "document_metadata");

            migrationBuilder.DropTable(
                name: "login_history");

            migrationBuilder.DropTable(
                name: "logins_x_groups");

            migrationBuilder.DropTable(
                name: "rbac_group_sync");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "spent");

            migrationBuilder.DropTable(
                name: "tags_x_groups");

            migrationBuilder.DropTable(
                name: "tags_x_ingestions");

            migrationBuilder.DropTable(
                name: "tags_x_logins");

            migrationBuilder.DropTable(
                name: "tags_x_rbac_role_sync");

            migrationBuilder.DropTable(
                name: "task");

            migrationBuilder.DropTable(
                name: "token_cost");

            migrationBuilder.DropTable(
                name: "client_sso");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "ingestion");

            migrationBuilder.DropTable(
                name: "login");

            migrationBuilder.DropTable(
                name: "rbac_role_sync");

            migrationBuilder.DropTable(
                name: "tags");
        }
    }
}
