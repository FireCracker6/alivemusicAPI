using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALIVEMusicAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrackTableTrackFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackFilePath",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackFilePath",
                table: "Tracks");
        }
    }
}
