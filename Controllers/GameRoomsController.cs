using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/GameRoom")]
    [ApiController]
    public class GameRoomController(GameRoomsRepo repo) : ControllerBase
    {
        private readonly GameRoomsRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Player? admin)
        {       
            var result = await repo.GetAllGameRooms(admin);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var gameRoom = await repo.GetGameRoomById(id);
            if (gameRoom == null) return NotFound();
            return Ok(gameRoom);
        }
        

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GameRoom newGameRoom)
        {
            bool result = await repo.UpdateGameRoom(id,newGameRoom);
            return result? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveGameRoom([FromBody] int adminId)
        {
            // Retrieve the player by adminId
            var admin = await repo.GetPlayerById(adminId);
            if (admin == null)
            {
                return NotFound("Admin player not found.");
            }

            // Create a new GameRoom instance and assign properties
            var newGameRoom = new GameRoom
            {
                Admin = admin,
                RoomCode = new Random().Next(1000, 9999),
                Players = new List<Player> { admin }
            };

            // Save the new game room to the database
            var result = await repo.SaveGameRoomToDb(newGameRoom);
            return CreatedAtAction(nameof(SaveGameRoom), new { newGameRoom.Id }, result);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameRoom([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteGameRoomById(id);    
            if (!isDeleted)  return NotFound();
            return NoContent();
        }
        [HttpGet("Player/{playerId}")]
        public async Task<IActionResult> GetPlayerById([FromRoute] int playerId)
        {
            var player = await repo.GetPlayerById(playerId);
            if (player == null) return NotFound();
            return Ok(player);
        }
    }
}