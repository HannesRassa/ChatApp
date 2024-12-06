using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backEnd.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGamePls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Groups_GroupId1",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Rounds_RoundId1",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Lobbies_LobbyId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Games_GameId1",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_GameId1",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Groups_RoundId1",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Answers_GroupId1",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "RoundId1",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Answers");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Lobbies_LobbyId",
                table: "Players",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Lobbies_LobbyId",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "Rounds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoundId1",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupId1",
                table: "Answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_GameId1",
                table: "Rounds",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_RoundId1",
                table: "Groups",
                column: "RoundId1");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_GroupId1",
                table: "Answers",
                column: "GroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Groups_GroupId1",
                table: "Answers",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Rounds_RoundId1",
                table: "Groups",
                column: "RoundId1",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Lobbies_LobbyId",
                table: "Players",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Games_GameId1",
                table: "Rounds",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
