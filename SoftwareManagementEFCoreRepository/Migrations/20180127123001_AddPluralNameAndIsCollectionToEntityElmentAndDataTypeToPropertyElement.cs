using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddPluralNameAndIsCollectionToEntityElmentAndDataTypeToPropertyElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "PropertyElementStates",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCollection",
                table: "EntityElementStates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PluralName",
                table: "EntityElementStates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataType",
                table: "PropertyElementStates");

            migrationBuilder.DropColumn(
                name: "IsCollection",
                table: "EntityElementStates");

            migrationBuilder.DropColumn(
                name: "PluralName",
                table: "EntityElementStates");
        }
    }
}
