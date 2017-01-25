using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddProductDescriptionAndCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessCase",
                table: "ProductStates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductStates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessCase",
                table: "ProductStates");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductStates");
        }
    }
}
