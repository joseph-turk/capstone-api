using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CapstoneApi.Migrations
{
    public partial class UseGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events"
            );

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Events"
            );

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Events",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events"
            );

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Events"
            );

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Events",
                nullable: false
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id"
            );
        }
    }
}
