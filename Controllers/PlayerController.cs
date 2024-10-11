using BackEnd.Data;
using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Player")]
    [ApiController]
    public class PlayerController(PlayerRepo repo) : ControllerBase
    {
        private readonly PlayerRepo repo = repo;

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Player newPlayer)
        {
            if (newPlayer == null)
                return BadRequest();

            await repo.AddPlayer(newPlayer);
            return CreatedAtAction(nameof(GetById), new { id = newPlayer.PlayerID }, newPlayer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Player updatedPlayer)
        {
            if (updatedPlayer == null || updatedPlayer.PlayerID != id)
                return BadRequest();

            var existingPlayer = await repo.GetPlayerById(id);
            if (existingPlayer == null)
                return NotFound();

            await repo.UpdatePlayer(updatedPlayer);
            return NoContent();
        }

    }
}