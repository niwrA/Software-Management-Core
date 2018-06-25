using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddTenantIdToCommandStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EntityRootGuid",
                table: "CommandStates",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<string>(
                name: "EntityGuid",
                table: "CommandStates",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CommandStates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CommandStates");

            migrationBuilder.AlterColumn<Guid>(
                name: "EntityRootGuid",
                table: "CommandStates",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EntityGuid",
                table: "CommandStates",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
