using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BenchmarkApp.Server.Migrations
{
    public partial class AddedWriteTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WriteUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WriteFriendships",
                columns: table => new
                {
                    FriendAId = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendBId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WriteFriendships", x => new { x.FriendAId, x.FriendBId });
                    table.ForeignKey(
                        name: "FK_WriteFriendships_WriteUsers_FriendAId",
                        column: x => x.FriendAId,
                        principalTable: "WriteUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WriteFriendships_WriteUsers_FriendBId",
                        column: x => x.FriendBId,
                        principalTable: "WriteUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WriteFriendships_FriendAId",
                table: "WriteFriendships",
                column: "FriendAId");

            migrationBuilder.CreateIndex(
                name: "IX_WriteFriendships_FriendBId",
                table: "WriteFriendships",
                column: "FriendBId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WriteFriendships");

            migrationBuilder.DropTable(
                name: "WriteUsers");
        }
    }
}
