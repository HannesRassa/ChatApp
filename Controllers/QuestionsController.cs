using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Question")]
    [ApiController]
    public class QuestionController(QuestionsRepo repo) : ControllerBase
    {
        private readonly QuestionsRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {       
            var result = await repo.GetAllQuestions();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var question = await repo.GetQuestionById(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Question newQuestion)
        {
            bool result = await repo.UpdateQuestion(id,newQuestion);
            return result? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuestion([FromBody] Question newQuesiton)
        {
            var questionExists = await repo.QuestionExistsInDb(newQuesiton.Id);
            if (questionExists) return Conflict();

            var result = await repo.SaveQuestionToDb(newQuesiton);

            return CreatedAtAction(nameof(SaveQuestion), new { newQuesiton.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteQuestionById(id);    
            if (!isDeleted)  return NotFound();
            return NoContent();
        }
    }
}
