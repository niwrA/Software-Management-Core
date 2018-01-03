using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddAccountAndDatabaseToEnvironment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyEnvironmentAccountStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CompanyGuid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EnvironmentGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEnvironmentAccountStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CompanyEnvironmentAccountStates_CompanyEnvironmentStates_EnvironmentGuid",
                        column: x => x.EnvironmentGuid,
                        principalTable: "CompanyEnvironmentStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyEnvironmentDatabaseStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CompanyGuid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EnvironmentGuid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEnvironmentDatabaseStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CompanyEnvironmentDatabaseStates_CompanyEnvironmentStates_EnvironmentGuid",
                        column: x => x.EnvironmentGuid,
                        principalTable: "CompanyEnvironmentStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEnvironmentAccountStates_EnvironmentGuid",
                table: "CompanyEnvironmentAccountStates",
                column: "EnvironmentGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEnvironmentDatabaseStates_EnvironmentGuid",
                table: "CompanyEnvironmentDatabaseStates",
                column: "EnvironmentGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyEnvironmentAccountStates");

            migrationBuilder.DropTable(
                name: "CompanyEnvironmentDatabaseStates");
        }
    }
}
