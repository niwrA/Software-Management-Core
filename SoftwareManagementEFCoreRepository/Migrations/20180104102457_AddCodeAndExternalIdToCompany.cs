using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddCodeAndExternalIdToCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CompanyStates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "CompanyStates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "CompanyStates");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "CompanyStates");
        }
    }
}
