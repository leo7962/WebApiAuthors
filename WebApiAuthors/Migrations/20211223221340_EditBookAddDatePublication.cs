using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAuthors.Migrations
{
    public partial class EditBookAddDatePublication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublicationDate",
                table: "Books",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicationDate",
                table: "Books");
        }
    }
}
