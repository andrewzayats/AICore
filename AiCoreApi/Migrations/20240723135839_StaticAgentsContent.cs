using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class StaticAgentsContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 7, 23, 13, 58, 38, 660, DateTimeKind.Utc).AddTicks(3128));

            migrationBuilder.Sql(
                @"do $$
                declare
                    agent_record record;
                    old_content jsonb;
                    new_content jsonb;
                    name text;
                    key text;
                    value text;
                begin
                    for agent_record in select agent_id, content from agents loop
                        old_content := agent_record.content;
                        new_content := '{}';
                        for key, value in select * from jsonb_each_text(old_content) loop
                            name := initcap(regexp_replace(regexp_replace(key, '([A-Z])', ' \1','g'), '^ ', ''));
                            new_content := jsonb_set(new_content, array[key], jsonb_build_object('Name', name, 'Code', key, 'Value', value));
                        end loop;
                        update 
				                agents
        	                set 
				                content = new_content
                        where 
			                agent_id = agent_record.agent_id;
                    end loop;
                end $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "login",
                keyColumn: "login_id",
                keyValue: 1,
                column: "created",
                value: new DateTime(2024, 7, 4, 13, 18, 26, 293, DateTimeKind.Utc).AddTicks(1609));
        }
    }
}
