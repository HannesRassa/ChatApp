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

        [HttpGet("ByPack/{packName}")]
        public async Task<IActionResult> GetByPackName([FromRoute] string packName)
        {
            var questions = await repo.GetQuestionsByPackName(packName);
            if (questions == null || !questions.Any()) return NotFound();
            return Ok(questions);
        }

        [HttpGet("Packs")]
        public async Task<IActionResult> GetAllPackNames()
        {
            var packNames = await repo.GetDistinctPackNames();
            if (!packNames.Any()) return NotFound();
            return Ok(packNames);
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuestion([FromBody] Question newQuestion)
        {
            var questionExists = await repo.QuestionExistsInDb(newQuestion.Id);
            if (questionExists) return Conflict();

            var result = await repo.SaveQuestionToDb(newQuestion);
            return CreatedAtAction(nameof(SaveQuestion), new { newQuestion.Id }, result);
        }

        [HttpPost("Batch")]
        public async Task<IActionResult> SaveQuestionsBatch([FromBody] IEnumerable<Question> questions)
        {
            if (!questions.Any()) return BadRequest("The questions list is empty.");

            var result = await repo.SaveQuestionsToDb(questions);
            return Created(nameof(SaveQuestionsBatch), result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Question newQuestion)
        {
            bool result = await repo.UpdateQuestion(id, newQuestion);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteQuestionById(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
    }
}
