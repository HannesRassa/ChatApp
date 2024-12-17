using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backEnd.Migrations
{
    /// <inheritdoc />
    public partial class CreateLobbiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LobbyPlayers_Lobbies_LobbyId",
                table: "LobbyPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_LobbyPlayers_Players_PlayersId",
                table: "LobbyPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LobbyPlayers",
                table: "LobbyPlayers");

            migrationBuilder.RenameTable(
                name: "LobbyPlayers",
                newName: "LobbyPlayer");

            migrationBuilder.RenameIndex(
                name: "IX_LobbyPlayers_PlayersId",
                table: "LobbyPlayer",
                newName: "IX_LobbyPlayer_PlayersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LobbyPlayer",
                table: "LobbyPlayer",
                columns: new[] { "LobbyId", "PlayersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LobbyPlayer_Lobbies_LobbyId",
                table: "LobbyPlayer",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LobbyPlayer_Players_PlayersId",
                table: "LobbyPlayer",
                column: "PlayersId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LobbyPlayer_Lobbies_LobbyId",
                table: "LobbyPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_LobbyPlayer_Players_PlayersId",
                table: "LobbyPlayer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LobbyPlayer",
                table: "LobbyPlayer");

            migrationBuilder.RenameTable(
                name: "LobbyPlayer",
                newName: "LobbyPlayers");

            migrationBuilder.RenameIndex(
                name: "IX_LobbyPlayer_PlayersId",
                table: "LobbyPlayers",
                newName: "IX_LobbyPlayers_PlayersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LobbyPlayers",
                table: "LobbyPlayers",
                columns: new[] { "LobbyId", "PlayersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LobbyPlayers_Lobbies_LobbyId",
                table: "LobbyPlayers",
                column: "LobbyId",
                principalTable: "Lobbies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LobbyPlayers_Players_PlayersId",
                table: "LobbyPlayers",
                column: "PlayersId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
