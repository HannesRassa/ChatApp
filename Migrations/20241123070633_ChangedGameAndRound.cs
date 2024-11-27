using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backEnd.Migrations
{
    /// <inheritdoc />
    public partial class ChangedGameAndRound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Games_GameId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Rounds_RoundId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_GameId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_RoundId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Players_GameId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "RoundId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayersPerGroup",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "RoundsAmount",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TimerForAnsweringInSec",
                table: "Games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Questions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundId",
                table: "Questions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayersPerGroup",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoundsAmount",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimerForAnsweringInSec",
                table: "Games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                column: "GameId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                column: "GameId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GameId", "RoundId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GameId", "RoundId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GameId",
                table: "Questions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_RoundId",
                table: "Questions",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Games_GameId",
                table: "Questions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Rounds_RoundId",
                table: "Questions",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id");
        }
    }
}
