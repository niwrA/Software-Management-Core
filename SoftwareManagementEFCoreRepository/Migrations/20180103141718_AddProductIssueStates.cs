using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddProductIssueStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductIssueState_ProductStates_ProductGuid",
                table: "ProductIssueState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductIssueState",
                table: "ProductIssueState");

            migrationBuilder.RenameTable(
                name: "ProductIssueState",
                newName: "ProductIssueStates");

            migrationBuilder.RenameIndex(
                name: "IX_ProductIssueState_ProductGuid",
                table: "ProductIssueStates",
                newName: "IX_ProductIssueStates_ProductGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductIssueStates",
                table: "ProductIssueStates",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIssueStates_ProductStates_ProductGuid",
                table: "ProductIssueStates",
                column: "ProductGuid",
                principalTable: "ProductStates",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductIssueStates_ProductStates_ProductGuid",
                table: "ProductIssueStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductIssueStates",
                table: "ProductIssueStates");

            migrationBuilder.RenameTable(
                name: "ProductIssueStates",
                newName: "ProductIssueState");

            migrationBuilder.RenameIndex(
                name: "IX_ProductIssueStates_ProductGuid",
                table: "ProductIssueState",
                newName: "IX_ProductIssueState_ProductGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductIssueState",
                table: "ProductIssueState",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIssueState_ProductStates_ProductGuid",
                table: "ProductIssueState",
                column: "ProductGuid",
                principalTable: "ProductStates",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
