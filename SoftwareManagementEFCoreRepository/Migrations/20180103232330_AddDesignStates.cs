using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddDesignStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DesignElementStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DesignGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignElementStates", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "DesignStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignStates", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "EpicElementStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DesignGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpicElementStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_EpicElementStates_DesignStates_DesignGuid",
                        column: x => x.DesignGuid,
                        principalTable: "DesignStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityElementStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DesignGuid = table.Column<Guid>(nullable: false),
                    EpicElementGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityElementStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_EntityElementStates_EpicElementStates_EpicElementGuid",
                        column: x => x.EpicElementGuid,
                        principalTable: "EpicElementStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommandElementStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DesignGuid = table.Column<Guid>(nullable: false),
                    EntityElementGuid = table.Column<Guid>(nullable: false),
                    EpicElementGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandElementStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CommandElementStates_EntityElementStates_EntityElementGuid",
                        column: x => x.EntityElementGuid,
                        principalTable: "EntityElementStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyElementStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DesignGuid = table.Column<Guid>(nullable: false),
                    EntityElementGuid = table.Column<Guid>(nullable: false),
                    EpicElementGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyElementStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_PropertyElementStates_EntityElementStates_EntityElementGuid",
                        column: x => x.EntityElementGuid,
                        principalTable: "EntityElementStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommandElementStates_EntityElementGuid",
                table: "CommandElementStates",
                column: "EntityElementGuid");

            migrationBuilder.CreateIndex(
                name: "IX_EntityElementStates_EpicElementGuid",
                table: "EntityElementStates",
                column: "EpicElementGuid");

            migrationBuilder.CreateIndex(
                name: "IX_EpicElementStates_DesignGuid",
                table: "EpicElementStates",
                column: "DesignGuid");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyElementStates_EntityElementGuid",
                table: "PropertyElementStates",
                column: "EntityElementGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandElementStates");

            migrationBuilder.DropTable(
                name: "DesignElementStates");

            migrationBuilder.DropTable(
                name: "PropertyElementStates");

            migrationBuilder.DropTable(
                name: "EntityElementStates");

            migrationBuilder.DropTable(
                name: "EpicElementStates");

            migrationBuilder.DropTable(
                name: "DesignStates");
        }
    }
}
