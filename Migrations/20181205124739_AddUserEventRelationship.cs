using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CapstoneApi.Migrations
{
    public partial class AddUserEventRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedById",
                table: "Events",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_CreatedById",
                table: "Events",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_CreatedById",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CreatedById",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Events");
        }
    }
}
