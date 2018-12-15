using Microsoft.EntityFrameworkCore.Migrations;

namespace TP2_ASP.Data.Migrations
{
    public partial class modifySender : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SendViewModel",
                table: "SendViewModel");

            migrationBuilder.RenameTable(
                name: "SendViewModel",
                newName: "SendViewModels");

            migrationBuilder.AlterColumn<string>(
                name: "Sender",
                table: "SendViewModels",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SendViewModels",
                table: "SendViewModels",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SendViewModels",
                table: "SendViewModels");

            migrationBuilder.RenameTable(
                name: "SendViewModels",
                newName: "SendViewModel");

            migrationBuilder.AlterColumn<int>(
                name: "Sender",
                table: "SendViewModel",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SendViewModel",
                table: "SendViewModel",
                column: "Id");
        }
    }
}
