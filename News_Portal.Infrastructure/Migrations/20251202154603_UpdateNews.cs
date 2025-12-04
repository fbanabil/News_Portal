using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace News_Portal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewsType",
                schema: "News_Portal",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewsType",
                schema: "News_Portal",
                table: "News");
        }
    }
}
