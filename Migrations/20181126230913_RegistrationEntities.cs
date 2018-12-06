using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CapstoneApi.Migrations
{
    public partial class RegistrationEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Registrants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhotoRelease = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EventId = table.Column<Guid>(nullable: true),
                    PrimaryContactId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_Registrants_PrimaryContactId",
                        column: x => x.PrimaryContactId,
                        principalTable: "Registrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_EventId",
                table: "Registrations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_PrimaryContactId",
                table: "Registrations",
                column: "PrimaryContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrantRegistration");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Registrants");
        }
    }
}
