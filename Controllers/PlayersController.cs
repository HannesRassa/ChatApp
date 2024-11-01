using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Player")]
    [ApiController]
    public class PlayerController(PlayersRepo repo) : ControllerBase
    {
        private readonly PlayersRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {       
            var result = await repo.GetAllPlayers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var player = await repo.GetPlayerById(id);
            if (player == null)
                return NotFound();
            return Ok(player);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Player newPlayer)
        {
            bool result = await repo.UpdatePlayer(id,newPlayer);
            return result? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SavePlayer([FromBody] Player newPlayer)
        {
            var playerExists = await repo.PlayerExistsInDb(newPlayer.Id);
            if (playerExists) return Conflict();
            var result = repo.SavePlayerToDb(newPlayer);
            return CreatedAtAction(nameof(SavePlayer), new { newPlayer.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer([FromRoute] int id)
        {
            bool isDeleted = await repo.DeletePlayerById(id);    
            if (!isDeleted)  return NotFound();
            return NoContent();
        }
    }
}