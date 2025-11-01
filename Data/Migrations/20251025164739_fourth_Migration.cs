using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace S_DES_project4.Data.Migrations
{
    public partial class fourth_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyValue",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyValue",
                table: "Friends");
        }
    }
}
