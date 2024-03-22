using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALIVEMusicAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Artists_ArtistID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment_ParentCommentID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Tracks_TrackID",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserID",
                table: "Comments",
                newName: "IX_Comments_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_TrackID",
                table: "Comments",
                newName: "IX_Comments_TrackID");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ParentCommentID",
                table: "Comments",
                newName: "IX_Comments_ParentCommentID");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ArtistID",
                table: "Comments",
                newName: "IX_Comments_ArtistID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "CommentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Artists_ArtistID",
                table: "Comments",
                column: "ArtistID",
                principalTable: "Artists",
                principalColumn: "ArtistID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserID",
                table: "Comments",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentID",
                table: "Comments",
                column: "ParentCommentID",
                principalTable: "Comments",
                principalColumn: "CommentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tracks_TrackID",
                table: "Comments",
                column: "TrackID",
                principalTable: "Tracks",
                principalColumn: "TrackID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Artists_ArtistID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tracks_TrackID",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserID",
                table: "Comment",
                newName: "IX_Comment_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TrackID",
                table: "Comment",
                newName: "IX_Comment_TrackID");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParentCommentID",
                table: "Comment",
                newName: "IX_Comment_ParentCommentID");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ArtistID",
                table: "Comment",
                newName: "IX_Comment_ArtistID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "CommentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Artists_ArtistID",
                table: "Comment",
                column: "ArtistID",
                principalTable: "Artists",
                principalColumn: "ArtistID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserID",
                table: "Comment",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment_ParentCommentID",
                table: "Comment",
                column: "ParentCommentID",
                principalTable: "Comment",
                principalColumn: "CommentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Tracks_TrackID",
                table: "Comment",
                column: "TrackID",
                principalTable: "Tracks",
                principalColumn: "TrackID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
