using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddCompanyEnvironmentHardware : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "EmploymentStates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarFileGuid",
                table: "ContactStates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "ContactStates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyEnvironmentStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEnvironmentStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CompanyEnvironmentStates_CompanyStates_CompanyGuid",
                        column: x => x.CompanyGuid,
                        principalTable: "CompanyStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFeatureStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstVersionGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRequest = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedForVersionGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeatureStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductFeatureStates_ProductStates_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "ProductStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductIssueState",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstVersionGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIssueState", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductIssueState_ProductStates_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "ProductStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVersionStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Build = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Major = table.Column<int>(type: "int", nullable: false),
                    Minor = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVersionStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductVersionStates_ProductStates_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "ProductStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRoleAssignmentStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectRoleGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoleAssignmentStates", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "CompanyEnvironmentHardwareStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnvironmentGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEnvironmentHardwareStates", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CompanyEnvironmentHardwareStates_CompanyEnvironmentStates_EnvironmentGuid",
                        column: x => x.EnvironmentGuid,
                        principalTable: "CompanyEnvironmentStates",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEnvironmentHardwareStates_EnvironmentGuid",
                table: "CompanyEnvironmentHardwareStates",
                column: "EnvironmentGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEnvironmentStates_CompanyGuid",
                table: "CompanyEnvironmentStates",
                column: "CompanyGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatureStates_ProductGuid",
                table: "ProductFeatureStates",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductIssueState_ProductGuid",
                table: "ProductIssueState",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVersionStates_ProductGuid",
                table: "ProductVersionStates",
                column: "ProductGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyEnvironmentHardwareStates");

            migrationBuilder.DropTable(
                name: "ProductFeatureStates");

            migrationBuilder.DropTable(
                name: "ProductIssueState");

            migrationBuilder.DropTable(
                name: "ProductVersionStates");

            migrationBuilder.DropTable(
                name: "ProjectRoleAssignmentStates");

            migrationBuilder.DropTable(
                name: "CompanyEnvironmentStates");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "EmploymentStates");

            migrationBuilder.DropColumn(
                name: "AvatarFileGuid",
                table: "ContactStates");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "ContactStates");
        }
    }
}
