using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Game")]
    [ApiController]
    public class GameController(GameRepo gameRepo) : ControllerBase
    {
        private readonly GameRepo _gameRepo = gameRepo;

        // Start new game
        [HttpPost("Start")]
        public async Task<IActionResult> StartNewGame([FromBody] StartGameRequest request)
        {
            if (request.PlayerIds.Count == 0) return BadRequest("No players provided.");

            var gameRoom = await _gameRepo.StartNewGame(request.AdminId, request.PlayerIds);
            return Ok(gameRoom);
        }

        // Get curren question
        [HttpGet("{roomCode}/CurrentQuestion")]
        public async Task<IActionResult> GetCurrentQuestion([FromRoute] int roomCode)
        {
            var question = await _gameRepo.GetCurrentQuestion(roomCode);
            if (question == null) return NotFound("No current question or game not found.");
            return Ok(question);
        }

        // Recieve player`s answer
        [HttpPost("{roomCode}/Answer")]
        public async Task<IActionResult> SubmitAnswer([FromRoute] int roomCode, [FromBody] SubmitAnswerRequest request)
        {
            var currentQuestion = await _gameRepo.GetCurrentQuestion(roomCode);
            if (currentQuestion == null) return NotFound("No current question found.");

            // Проверка правильности ответа
            bool isCorrect = currentQuestion.CorrectAnswer == request.Answer;

            // Обновление очков игрока
            bool scoreUpdated = await _gameRepo.UpdatePlayerScore(roomCode, request.PlayerId, isCorrect);

            if (!scoreUpdated) return BadRequest("Failed to update player score.");
            return Ok(new { IsCorrect = isCorrect });
        }

        // Go to next question
        [HttpPost("{roomCode}/NextQuestion")]
public async Task<IActionResult> GoToNextQuestion([FromRoute] int roomCode)
{
    var gameRoom = await _gameRepo.GetGameRoomByCode(roomCode);
    if (gameRoom == null)
        return NotFound("Game room not found.");

    if (gameRoom.CurrentQuestion >= gameRoom.Questions.Count - 1)
        return BadRequest("No more questions available.");

    bool result = await _gameRepo.GoToNextQuestion(roomCode);
    if (!result) return BadRequest("Failed to move to the next question.");
    
    return Ok("Moved to the next question.");
}
    }

    // DTO for game start
    public record StartGameRequest
    {
        public int AdminId { get; init; }
        public List<int> PlayerIds { get; init; } = new();
    }

    // DTO for sending the answer
    public record SubmitAnswerRequest
    {
        public int PlayerId { get; init; }
        public string Answer { get; init; } = string.Empty;
    }
}
