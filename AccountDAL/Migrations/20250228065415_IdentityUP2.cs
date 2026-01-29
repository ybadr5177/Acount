using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountDAL.Migrations
{
    public partial class IdentityUP2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FristNames",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FristNames",
                table: "AspNetUsers");
        }
    }
}
