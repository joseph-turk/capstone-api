using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CapstoneApi.Migrations
{
    public partial class TweakImageSaving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Events",
                newName: "ImageName");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Events",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Events",
                newName: "ImageUrl");
        }
    }
}
