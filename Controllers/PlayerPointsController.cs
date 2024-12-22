using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/PlayerPoint")]
    [ApiController]
    public class PlayerPointController(PlayerPointsRepo repo) : ControllerBase
    {
        private readonly PlayerPointsRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await repo.GetAllPlayerPoints();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var playerPoint = await repo.GetPlayerPointById(id);
            if (playerPoint == null)
                return NotFound();
            return Ok(playerPoint);
        }

        [HttpGet("find-by-gameId/{gameId}")]
        public async Task<IActionResult> GetByGameId([FromRoute] int gameId)
        {
            if (gameId <= 0)
            {
                return BadRequest("Game ID must be a positive number.");
            }

            var playerPoints = await repo.GetAllPlayerPointsByGameId(gameId);
            if (playerPoints == null || !playerPoints.Any())
                return NotFound();

            return Ok(playerPoints);
        }
        // [HttpGet("find-by-PlayerId-and-gameId/{playerId}/{gameId}")]
        // public async Task<IActionResult> GetByPlayerIdAndGameId([FromRoute] int playerId, [FromRoute] int gameId)
        // {
        //      Console.WriteLine($"Received PlayerId: {playerId}, GameId: {gameId}"); // Debug line
        //     if (playerId <= 0 || gameId <= 0)
        //     {
        //         return BadRequest("Player ID and Game ID must be positive numbers.");
        //     }
        //     var playerPoint = await repo.GetPlayerPointByPlayerIdAndGameId(playerId, gameId);
        //     if (playerPoint == null || !playerPoint.Any())
        //         return NotFound();
        //     return Ok(playerPoint);
        // }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlayerPoint newPlayerPoint)
        {
            bool result = await repo.UpdatePlayerPoint(id, newPlayerPoint);
            return result ? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SavePlayerPoint([FromBody] PlayerPoint newPlayerPoint)
        {
            var playerPointExists = await repo.PlayerPointExistsInDb(newPlayerPoint.Id);
            if (playerPointExists) return Conflict();
            var result = await repo.SavePlayerPointToDb(newPlayerPoint);
            return CreatedAtAction(nameof(SavePlayerPoint), new { newPlayerPoint.Id }, result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerPoint([FromRoute] int id)
        {
            bool isDeleted = await repo.DeletePlayerPointById(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
    }
}