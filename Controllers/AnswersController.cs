using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Answer")]
    [ApiController]
    public class AnswerController(AnswersRepo repo) : ControllerBase
    {
        private readonly AnswersRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Question? question, [FromQuery] Player? palyer)
        {       
            var result = await repo.GetAllAnswers(question, player);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var answer = await repo.GetAnswerById(id);
            if (answer == null)  return NotFound();
            return Ok(answer);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAnswer([FromBody] Answer newAnswer)
        {
            var answerExists = await repo.AnswerExistsInDb(newAnswer.Id);
            if (answerExists) return Conflict();
            var result = repo.SaveAnswerToDb(newAnswer);
            return CreatedAtAction(nameof(SaveAnswer), new { newAnswer.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Answer newAnswer)
        {
            bool result = await repo.UpdateAnswer(id,newAnswer);
            return result? NoContent() : NotFound();
        }
    }
}