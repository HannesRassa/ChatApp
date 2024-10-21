using BackEnd.Data;
using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using ISA3Demos.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Question")]
    [ApiController]
    public class QuestionController(QuestionsRepo repo) : ControllerBase
    {
        private readonly QuestionsRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QuestionHardness? hardness)
        {       
            var result = await repo.GetAllQuestions(hardness);
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
        public async Task<IActionResult> SaveQuestion([FromBody] Question newQuesiton)
        {
            var questionExists = await repo.QuestionExistsInDb(newQuesiton.QuestionID);

            if (questionExists) return Conflict();

            var result = repo.SaveQuestionToDb(newQuesiton);
            return CreatedAtAction(nameof(SaveQuestion), new { newQuesiton.QuestionID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Question newQuestion)
        {
            bool result = await repo.UpdateQuestion(id,newQuestion);
            return result? NoContent() : NotFound();
        }
    }
}