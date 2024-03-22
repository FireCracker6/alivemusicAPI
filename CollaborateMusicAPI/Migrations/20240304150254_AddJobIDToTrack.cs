using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALIVEMusicAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddJobIDToTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JobID",
                table: "Tracks",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobID",
                table: "Tracks");
        }
    }
}
