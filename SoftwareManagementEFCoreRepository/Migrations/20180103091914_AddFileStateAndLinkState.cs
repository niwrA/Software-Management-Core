using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    public partial class AddFileStateAndLinkState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EntityGuid = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    ForGuid = table.Column<Guid>(nullable: false),
                    ForType = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStates", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "LinkStates",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EntityGuid = table.Column<Guid>(nullable: false),
                    ForGuid = table.Column<Guid>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SiteName = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkStates", x => x.Guid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileStates_ForGuid",
                table: "FileStates",
                column: "ForGuid");

            migrationBuilder.CreateIndex(
                name: "IX_LinkStates_ForGuid",
                table: "LinkStates",
                column: "ForGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileStates");

            migrationBuilder.DropTable(
                name: "LinkStates");
        }
    }
}
