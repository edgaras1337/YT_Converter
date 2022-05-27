using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class TablesUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "DownloadedVideos");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "DownloadedAudios");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "DownloadedVideos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "DownloadedAudios",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
