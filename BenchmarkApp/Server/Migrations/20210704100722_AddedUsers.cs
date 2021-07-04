using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                    FriendAId = table.Column<int>(type: "integer", nullable: false),
                    FriendBId = table.Column<int>(type: "integer", nullable: false),
                    SqlUserEntityId = table.Column<int>(type: "integer", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Friendships_Users_SqlUserEntityId",
                        column: x => x.SqlUserEntityId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendBId",
                table: "Friendships",
                column: "FriendBId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_SqlUserEntityId",
                table: "Friendships",
                column: "SqlUserEntityId");
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
