using Microsoft.EntityFrameworkCore.Migrations;

namespace CapstoneApi.Migrations
{
    public partial class SpecifyCascades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_PrimaryContacts_PrimaryContactId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Registrants_RegistrantId",
                table: "Registrations");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_PrimaryContacts_PrimaryContactId",
                table: "Registrations",
                column: "PrimaryContactId",
                principalTable: "PrimaryContacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Registrants_RegistrantId",
                table: "Registrations",
                column: "RegistrantId",
                principalTable: "Registrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_PrimaryContacts_PrimaryContactId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Registrants_RegistrantId",
                table: "Registrations");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Events_EventId",
                table: "Registrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_PrimaryContacts_PrimaryContactId",
                table: "Registrations",
                column: "PrimaryContactId",
                principalTable: "PrimaryContacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Registrants_RegistrantId",
                table: "Registrations",
                column: "RegistrantId",
                principalTable: "Registrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
