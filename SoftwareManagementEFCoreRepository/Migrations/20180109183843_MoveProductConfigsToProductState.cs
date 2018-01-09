using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class MoveProductConfigsToProductState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFeatureConfigOptionStates");

            migrationBuilder.CreateTable(
                name: "ProductConfigOptionStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsDefaultOption = table.Column<bool>(nullable: false),
                    IsOptionForParent = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ParentGuid = table.Column<Guid>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    ProductFeatureGuid = table.Column<Guid>(nullable: false),
                    ProductGuid = table.Column<Guid>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConfigOptionStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductConfigOptionStates_ProductStates_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "ProductStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductConfigOptionStates_ProductGuid",
                table: "ProductConfigOptionStates",
                column: "ProductGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductConfigOptionStates");

            migrationBuilder.CreateTable(
                name: "ProductFeatureConfigOptionStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsDefaultOption = table.Column<bool>(nullable: false),
                    IsOptionForParent = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ParentGuid = table.Column<Guid>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    ProductFeatureGuid = table.Column<Guid>(nullable: false),
                    ProductGuid = table.Column<Guid>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeatureConfigOptionStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductFeatureConfigOptionStates_ProductFeatureStates_ProductFeatureGuid",
                        column: x => x.ProductFeatureGuid,
                        principalTable: "ProductFeatureStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatureConfigOptionStates_ProductFeatureGuid",
                table: "ProductFeatureConfigOptionStates",
                column: "ProductFeatureGuid");
        }
    }
}
