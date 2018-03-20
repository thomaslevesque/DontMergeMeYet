using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DontMergeMeYet.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepositoryInstallations",
                columns: table => new
                {
                    RepositoryId = table.Column<int>(nullable: false),
                    InstallationDate = table.Column<DateTime>(nullable: false),
                    InstallationId = table.Column<int>(nullable: false),
                    RepositoryFullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryInstallations", x => x.RepositoryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepositoryInstallations_InstallationId",
                table: "RepositoryInstallations",
                column: "InstallationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepositoryInstallations");
        }
    }
}
