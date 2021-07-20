using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BenchmarkApp.Server.Migrations
{
    public partial class AddedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    FriendAId = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendBId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => new { x.FriendAId, x.FriendBId });
                    table.ForeignKey(
                        name: "FK_Friendships_Users_FriendAId",
                        column: x => x.FriendAId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_FriendBId",
                        column: x => x.FriendBId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendBId",
                table: "Friendships",
                column: "FriendBId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
