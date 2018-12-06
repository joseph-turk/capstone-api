using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CapstoneApi.Migrations
{
    public partial class MoreEventData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "End",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Events");
        }
    }
}
