using BackEnd.Data;
using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Questiom")]
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
            if (question == null)
                return NotFound();
            return Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Question newQuestion)
        {
            if (newQuestion == null)
                return BadRequest();

            await repo.AddQuestion(newQuestion);
            return CreatedAtAction(nameof(GetById), new { id = newQuestion.QuestionID }, newQuestion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Question updatedQuestion)
        {
            if (updatedQuestion == null || updatedQuestion.QuestionID != id)
                return BadRequest();

            var existingQuestion = await repo.GetQuestionById(id);
            if (existingQuestion == null)
                return NotFound();

            await repo.UpdateQuestion(updatedQuestion);
            return NoContent();
        }

    }
}