    using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Game newGame)
        {
            bool result = await repo.UpdateGame(id,newGame);
            return result? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveGame([FromBody] Game newGame)
        {
            var gameExists = await repo.GameExistsInDb(newGame.Id);
            if (gameExists) return Conflict();
            var result = repo.SaveGameToDb(newGame);
            return CreatedAtAction(nameof(SaveGame), new { newGame.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteGameById(id);    
            if (!isDeleted)  return NotFound();
            return NoContent();
        }
    }
}