using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("Backend/Lobby")]
    public class LobbyController(LobbiesRepo repo) : ControllerBase
    {
        private readonly LobbiesRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await repo.GetAllLobbies();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lobby = await repo.GetLobbyById(id);
            if (lobby == null) return NotFound();
            return Ok(lobby);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Lobby lobby)
        {
            bool result = await repo.UpdateLobby(id, lobby);
            return result ? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveLobby([FromBody] Lobby lobby)
        {
            var lobbyExists = await repo.LobbyExistsInDb(lobby.Id);
            if (lobbyExists) return Conflict();
            var result = await repo.SaveLobbyToDb(lobby);
            return CreatedAtAction(nameof(GetById), new { id = lobby.Id }, result);
        }        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLobby([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteLobbyById(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }        
    }
}   