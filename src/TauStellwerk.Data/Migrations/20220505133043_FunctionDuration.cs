// <auto-generated/>
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TauStellwerk.Server.Database.Migrations
{
    public partial class FunctionDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "DccFunction",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "DccFunction");
        }
    }
}
