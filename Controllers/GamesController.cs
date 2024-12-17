using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("Backend/Game")]
    [ApiController]
    public class GameController(GamesRepo repo) : ControllerBase
    {
        private readonly GamesRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await repo.GetAllGames();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var game = await repo.GetGameById(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        [HttpGet("find-group/{gameId}/{playerId}")]
        public async Task<IActionResult> FindRoundAndGroupByGameIdAndPlayerId([FromRoute] int gameId, [FromRoute] int playerId)
        {
            if (gameId <= 0 || playerId <= 0)
            {
                return BadRequest("Invalid game ID or player ID.");
            }

            // Fetch round and group information using the repository method
            var (roundId, groupId) = await repo.FindRoundAndGroupByGameIdAndPlayerId(gameId, playerId);

            if (roundId == null || groupId == null)
            {
                return NotFound($"No group found for player ID {playerId} in game ID {gameId}.");
            }

            // Return both the round ID and group ID as a JSON object
            return Ok(new { roundId, groupId });
        }

        [HttpGet("Player/{playerId}")]
        public async Task<IActionResult> GetGameIdByPlayerId([FromRoute] int playerId)
        {
            try
            {
                // Fetch the game room for the player
                var gameRoom = await repo.GetGameByPlayerId(playerId);

                // If the game room is null, return a 404 Not Found
                if (gameRoom == null)
                {
                    return NotFound($"No room found for player with ID {playerId}");
                }

                // Return the room's ID (or RoomCode, depending on what you want to return)
                return Ok(gameRoom.Id); // Or gameRoom.RoomCode if that's what you prefer to return
            }
            catch
            {
                // Return a generic server error message if something goes wrong
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Game newGame)
        {
            bool result = await repo.UpdateGame(id, newGame);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] Game newGame)
        {
            try
            {
                var createdGame = await repo.SaveGameToDb(newGame);
                return CreatedAtAction(nameof(CreateGame), new { id = createdGame.Id }, createdGame);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveGame([FromBody] Game newGame)
        {
            var gameExists = await repo.GameExistsInDb(newGame.Id);
            if (gameExists) return Conflict();
            var result = await repo.SaveGameToDb(newGame);
            return CreatedAtAction(nameof(SaveGame), new { newGame.Id }, result);
        }
        [HttpPost("{id}/add-round")]
        public async Task<IActionResult> AddRound(int id, [FromBody] Round newRound)
        {
            var updatedGame = await repo.AddRoundToGame(id, newRound);
            if (updatedGame == null)
                return NotFound($"Game with ID {id} not found.");
            return Ok(updatedGame);
        }

        



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteGameById(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
    }
}