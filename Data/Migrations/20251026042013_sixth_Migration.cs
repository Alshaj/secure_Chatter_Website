using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S_DES_project4.Data.Migrations
{
    public partial class sixth_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BlockDateByA",
                table: "Friends",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BlockDateByB",
                table: "Friends",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BlockedByA",
                table: "Friends",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockedByB",
                table: "Friends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockDateByA",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "BlockDateByB",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "BlockedByA",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "BlockedByB",
                table: "Friends");
        }
    }
}
