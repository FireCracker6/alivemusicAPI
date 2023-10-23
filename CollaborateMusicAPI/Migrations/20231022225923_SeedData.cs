using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollaborateMusicAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Email", "OAuthId", "OAuthProvider", "PasswordHash" },
                values: new object[] { 1, new DateTime(2023, 10, 22, 22, 59, 23, 506, DateTimeKind.Utc).AddTicks(3196), "testuser@example.com", "OauthTest", "TestProvider", "TestPasswordHash" });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "UserProfileID", "Bio", "FullName", "Location", "ProfilePicturePath", "UserID", "WebsiteURL" },
                values: new object[] { 1, "This is a test bio.", "Test User", "Test City", null, 1, "https://example.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "UserProfileID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
