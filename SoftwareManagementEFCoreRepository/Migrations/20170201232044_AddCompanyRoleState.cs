using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddCompanyRoleState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyRoleStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CompanyGuid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRoleStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CompanyRoleStates_CompanyStates_CompanyGuid",
                        column: x => x.CompanyGuid,
                        principalTable: "CompanyStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRoleStates_CompanyGuid",
                table: "CompanyRoleStates",
                column: "CompanyGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyRoleStates");
        }
    }
}
