using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALIVEMusicAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Albums_AlbumID",
                table: "Tracks");

            migrationBuilder.AlterColumn<int>(
                name: "TrackNumber",
                table: "Tracks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Lyrics",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Tracks",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumID",
                table: "Tracks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ArtistID",
                table: "Tracks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ArtistID",
                table: "Tracks",
                column: "ArtistID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Albums_AlbumID",
                table: "Tracks",
                column: "AlbumID",
                principalTable: "Albums",
                principalColumn: "AlbumID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Artists_ArtistID",
                table: "Tracks",
                column: "ArtistID",
                principalTable: "Artists",
                principalColumn: "ArtistID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Albums_AlbumID",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Artists_ArtistID",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_ArtistID",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "ArtistID",
                table: "Tracks");

            migrationBuilder.AlterColumn<int>(
                name: "TrackNumber",
                table: "Tracks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lyrics",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Tracks",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AlbumID",
                table: "Tracks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Albums_AlbumID",
                table: "Tracks",
                column: "AlbumID",
                principalTable: "Albums",
                principalColumn: "AlbumID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
