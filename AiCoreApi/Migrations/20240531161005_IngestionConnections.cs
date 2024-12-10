using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class IngestionConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "connection",
                columns: table => new
                {
                    connection_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_connection", x => x.connection_id);
                });

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 5, 31, 16, 10, 5, 152, DateTimeKind.Utc).AddTicks(6912));

            migrationBuilder.Sql(
                @"with connection_params as (
	                select 
			                content->>'Site' as site,
			                content->>'Drive' as drive,
			                content->>'ClientId' as cientid,
			                content->>'TenantId' as tenantid,
			                content->>'ClientSecret' as clientsecret,
			                split_part(content->>'Site', '/', -1) as name
		                from 
			                ingestion
		                where 
			                content->>'Site' is not null
		                group by 1,2,3,4,5
                )
                insert into
		                connection (name, type, content, created, created_by)
	                select
			                name,
			                1,
			                format('{
                                ""Site"": ""%s"",
                                ""Drive"": ""%s"",
                                ""ClientId"": ""%s"",
                                ""TenantId"": ""%s"",
                                ""ClientSecret"": ""%s""
                            }', 
				            connection_params.site, 
				            connection_params.drive, 
				            connection_params.cientid, 
			                connection_params.tenantid, 
			                connection_params.clientsecret
		                    )::jsonb,
			                now(),
			                'admin@viacode.com'
		                from
                            connection_params");


            migrationBuilder.Sql(
                @"with connection_ids as (
	                select 
			                connection_id,
			                content->>'Site' as site
		                from 
			                connection
                )
                update 
		                ingestion
	                set 
		                content = format('{
				                  ""Path"": ""%s"",
				                  ""ConnectionId"": ""%s""
				                }', 
				                ingestion.content->>'Path', 
				                connection_ids.connection_id
		                    )::jsonb
	                from 
		                connection_ids
	                where 
		                ingestion.content->>'Site' is not null and
		                connection_ids.site = ingestion.content->>'Site'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "connection");

            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 5, 28, 18, 17, 58, 478, DateTimeKind.Utc).AddTicks(7893));
        }
    }
}
