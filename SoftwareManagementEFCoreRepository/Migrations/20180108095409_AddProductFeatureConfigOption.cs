using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddProductFeatureConfigOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductFeatureConfigOptionStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ParentGuid = table.Column<Guid>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    ProductFeatureGuid = table.Column<Guid>(nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFeatureConfigOptionStates");
        }
    }
}
