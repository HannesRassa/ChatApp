using BackEnd.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly DataContext _context;
        public PlayerController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            var players = _context.Players.ToList();
            return Ok(players);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var player = _context.Players.Find(id);
            if (player == null) return NotFound();
            return Ok(player);
        }

        
    }
}