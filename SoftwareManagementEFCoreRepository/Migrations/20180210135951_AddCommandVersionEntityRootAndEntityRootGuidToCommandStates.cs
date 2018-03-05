using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddCommandVersionEntityRootAndEntityRootGuidToCommandStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommandTypeId",
                table: "CommandStates",
                newName: "Command");

            migrationBuilder.AddColumn<string>(
                name: "EntityRoot",
                table: "CommandStates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommandVersion",
                table: "CommandStates",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityRootGuid",
                table: "CommandStates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityRoot",
                table: "CommandStates");

            migrationBuilder.DropColumn(
                name: "CommandVersion",
                table: "CommandStates");

            migrationBuilder.DropColumn(
                name: "EntityRootGuid",
                table: "CommandStates");

            migrationBuilder.RenameColumn(
                name: "Command",
                table: "CommandStates",
                newName: "CommandTypeId");
        }
    }
}
