using Microsoft.EntityFrameworkCore.Migrations;

namespace CapstoneApi.Migrations
{
    public partial class AddImageExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Events",
                newName: "ImageExtension");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageExtension",
                table: "Events",
                newName: "ImageName");
        }
    }
}
