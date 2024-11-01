using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Round")]
    [ApiController]
    public class RoundsController(RoundsRepo repo) : ControllerBase
    {
        private readonly RoundsRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {       
            var result = await repo.GetAllRounds();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var round = await repo.GetRoundById(id);
            if (round == null)
                return NotFound();
            return Ok(round);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Round newRound)
        {
            bool result = await repo.UpdateRound(id,newRound);
            return result? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveRound([FromBody] Round newRound)
        {
            var roundExists = await repo.RoundExistsInDb(newRound.Id);

            if (roundExists) return Conflict();

            var result = repo.SaveRoundToDb(newRound);
            return CreatedAtAction(nameof(SaveRound), new { newRound.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRound([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteRoundById(id);    
            if (!isDeleted)  return NotFound();
            return NoContent();
        }
    }
}