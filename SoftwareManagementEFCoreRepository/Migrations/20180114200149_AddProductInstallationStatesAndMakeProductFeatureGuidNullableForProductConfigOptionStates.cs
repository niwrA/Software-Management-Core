using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddProductInstallationStatesAndMakeProductFeatureGuidNullableForProductConfigOptionStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProductFeatureGuid",
                table: "ProductConfigOptionStates",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateTable(
                name: "ProductInstallationStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CompanyEnvironmentGuid = table.Column<Guid>(nullable: true),
                    CompanyGuid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ProductGuid = table.Column<Guid>(nullable: false),
                    ProductVersionGuid = table.Column<Guid>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInstallationStates", x => x.Guid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductInstallationStates");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductFeatureGuid",
                table: "ProductConfigOptionStates",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
