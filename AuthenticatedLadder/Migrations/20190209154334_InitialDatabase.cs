using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthenticatedLadder.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ladders",
                columns: table => new
                {
                    LadderId = table.Column<string>(nullable: false),
                    Platform = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Score = table.Column<long>(nullable: false),
                    EntryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ladders", x => new { x.LadderId, x.Platform, x.Username });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ladders");
        }
    }
}
