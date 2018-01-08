using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddOptionFieldsToProductFeatureConfigOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductFeatureConfigOptionStates",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultOption",
                table: "ProductFeatureConfigOptionStates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptionForParent",
                table: "ProductFeatureConfigOptionStates",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductFeatureConfigOptionStates");

            migrationBuilder.DropColumn(
                name: "IsDefaultOption",
                table: "ProductFeatureConfigOptionStates");

            migrationBuilder.DropColumn(
                name: "IsOptionForParent",
                table: "ProductFeatureConfigOptionStates");
        }
    }
}
