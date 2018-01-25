using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddEntityElementParentGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentGuid",
                table: "EntityElementStates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentGuid",
                table: "EntityElementStates");
        }
    }
}
