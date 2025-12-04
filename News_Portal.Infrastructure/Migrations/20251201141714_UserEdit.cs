using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace News_Portal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "News_Portal",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonImageUrl",
                schema: "News_Portal",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonImageUrl",
                schema: "News_Portal",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "News_Portal",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
