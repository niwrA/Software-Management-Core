using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddResolvedVersionToProductIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResolvedVersionGuid",
                table: "ProductIssueStates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolvedVersionGuid",
                table: "ProductIssueStates");
        }
    }
}
