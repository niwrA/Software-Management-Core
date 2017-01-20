using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddCreatedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "ProductStates",
                newName: "UpdatedOn");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ProductStates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ProductStates");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "ProductStates",
                newName: "Created");
        }
    }
}
