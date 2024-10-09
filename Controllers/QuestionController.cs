using BackEnd.Data;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Question")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly DataContext _context;
        public QuestionController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var questions = _context.Questions.ToList();
            return Ok(questions);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var question = _context.Questions.Find(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

         [HttpPost]
        public IActionResult Create([FromBody] Question newQuestion)
        {
            // Add the new question to the context
            _context.Questions.Add(newQuestion);
            
            // Save the changes to the database
            _context.SaveChanges();

            // Return the created question with a 201 status code
            return CreatedAtAction(nameof(GetById), new { id = newQuestion.QuestionID }, newQuestion);
        }
    }
}