using Microsoft.EntityFrameworkCore.Migrations;

namespace TP2_ASP.Data.Migrations
{
    public partial class addSender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sender",
                table: "SendViewModel",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sender",
                table: "SendViewModel");
        }
    }
}
