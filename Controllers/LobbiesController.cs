using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;
using static BackEnd.Models.Classes.Lobby;

namespace BackEnd.Controllers
{
    [Route("Backend/Lobby")]
    [ApiController]
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
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var lobby = await repo.GetLobbyById(id);
            if (lobby == null) return NotFound();
            return Ok(lobby);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Lobby newLobby)
        {
            bool result = await repo.UpdateLobby(id, newLobby);
            return result ? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveLobby([FromBody] int adminId)
        {
            // Retrieve the player by adminId
            var admin = await repo.GetPlayerById(adminId);
            if (admin == null)
            {
                return NotFound("Admin player not found.");
            }

            // Create a new Lobby instance and assign properties
            var newLobby = new Lobby
            {
                Admin = admin,
                RoomCode = new Random().Next(1000, 9999),
                Players = new List<Player> { admin }
            };

            // Save the new lobby to the database
            var result = await repo.SaveLobbyToDb(newLobby);
            return CreatedAtAction(nameof(SaveLobby), new { newLobby.Id }, result);
        }
        [HttpPost("JoinLobby")]
        public async Task<IActionResult> JoinLobby([FromBody] JoinLobbyRequest request)
        {
            var player = await repo.GetPlayerById(request.PlayerId);
            if (player == null) return NotFound("Player not found.");

            var lobby = await repo.GetLobbyByCode(request.RoomCode);
            if (lobby == null) return NotFound("Lobby room not found.");

            bool playerAlreadyInRoom = lobby.Players.Any(p => p.Id == request.PlayerId);
            if (playerAlreadyInRoom) return BadRequest("Player is already in the Lobby room.");

            lobby.Players.Add(player);
            await repo.UpdateLobby(lobby.Id, lobby);
            return Ok(lobby);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLobby([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteLobbyById(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
         [HttpGet("Player/{playerId}")]
        public async Task<IActionResult> GetLobbyIdByPlayerId([FromRoute] int playerId)
        {
            var lobbies = await repo.GetAllLobbies();

            var lobby = lobbies.FirstOrDefault(gr => gr.Players.Any(p => p.Id == playerId));

            if (lobby == null)
            {
                return NotFound(new { Message = "Player not found in any lobby." });
            }

            return Ok(new { LobbyId = lobby.Id });
        }

    }
}