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

        [HttpPost]
        public async Task<IActionResult> SaveGameRoom([FromBody] GameRoom newGameRoom)
        {
            var gameRoomExists = await repo.GameRoomExistsInDb(newGameRoom.Id);
            if (gameRoomExists) return Conflict();
            var result = repo.SaveGameRoomToDb(newGameRoom);
            return CreatedAtAction(nameof(SaveGameRoom), new { newGameRoom.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GameRoom newGameRoom)
        {
            bool result = await repo.UpdateGameRoom(id,newGameRoom);
            return result? NoContent() : NotFound();
        }
    }
}