using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommandStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CommandTypeId = table.Column<string>(nullable: true),
                    EntityGuid = table.Column<Guid>(nullable: false),
                    ExecutedOn = table.Column<DateTime>(nullable: true),
                    ParametersJson = table.Column<string>(nullable: true),
                    ReceivedOn = table.Column<DateTime>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandStates", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "ProductStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStates", x => x.Guid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommandStates");

            migrationBuilder.DropTable(
                name: "ProductStates");
        }
    }
}
