using Microsoft.EntityFrameworkCore.Migrations;

namespace BenchmarkApp.Server.Migrations
{
    public partial class addedIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendAId",
                table: "Friendships",
                column: "FriendAId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friendships_FriendAId",
                table: "Friendships");
        }
    }
}
