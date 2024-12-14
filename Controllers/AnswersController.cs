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
        public async Task<IActionResult> GetAll()
        {       
            var result = await repo.GetAllAnswers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var answer = await repo.GetAnswerById(id);
            if (answer == null)  return NotFound();
            return Ok(answer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Answer newAnswer)
        {
            bool result = await repo.UpdateAnswer(id,newAnswer);
            return result? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAnswer([FromBody] Answer newAnswer)
        {
            var answerExists = await repo.AnswerExistsInDb(newAnswer.Id);
            if (answerExists) return Conflict();
            var result = await repo.SaveAnswerToDb(newAnswer);
            return CreatedAtAction(nameof(SaveAnswer), new { newAnswer.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteAnswerById(id);    
            if (!isDeleted)  return NotFound();
            return NoContent();
        }
    }
}