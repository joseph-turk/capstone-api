using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CapstoneApi.Migrations
{
    public partial class RegistrationEntityUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Registrants_PrimaryContactId",
                table: "Registrations");

            migrationBuilder.DropTable(
                name: "RegistrantRegistration");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Registrants");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Registrants");

            migrationBuilder.DropColumn(
                name: "PhotoRelease",
                table: "Registrants");

            migrationBuilder.AddColumn<bool>(
                name: "HasPhotoRelease",
                table: "Registrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWaitList",
                table: "Registrations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RegistrantId",
                table: "Registrations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrimaryContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryContacts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_RegistrantId",
                table: "Registrations",
                column: "RegistrantId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_PrimaryContacts_PrimaryContactId",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Registrants_RegistrantId",
                table: "Registrations");

            migrationBuilder.DropTable(
                name: "PrimaryContacts");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_RegistrantId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "HasPhotoRelease",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "IsWaitList",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "RegistrantId",
                table: "Registrations");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Registrants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Registrants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhotoRelease",
                table: "Registrants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RegistrantRegistration",
                columns: table => new
                {
                    RegistrationId = table.Column<Guid>(nullable: false),
                    RegistrantId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrantRegistration", x => new { x.RegistrationId, x.RegistrantId });
                    table.ForeignKey(
                        name: "FK_RegistrantRegistration_Registrants_RegistrantId",
                        column: x => x.RegistrantId,
                        principalTable: "Registrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrantRegistration_Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrantRegistration_RegistrantId",
                table: "RegistrantRegistration",
                column: "RegistrantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Registrants_PrimaryContactId",
                table: "Registrations",
                column: "PrimaryContactId",
                principalTable: "Registrants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
