using BackEnd.Data;
using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Player")]
    [ApiController]
    public class PlayerController(PlayerRepo repo) : ControllerBase
    {
        private readonly PlayerRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await repo.GetAllPlayers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var player = await repo.GetPlayerById(id);
            if (player == null)
                return NotFound();
            return Ok(player);
        }

        [HttpPost]
        public async Task<IActionResult> SavePlayer([FromBody] Player newPlayer)
        {
            var playerExists = await repo.PlayerExtistsInDb(newPlayer.PlayerID);

            if (playerExists) return Conflict();

            var result = repo.SavePlayerToDb(newPlayer);
            return CreatedAtAction(nameof(SavePlayer), new { newPlayer.PlayerID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Player newPlayer)
        {
            bool result = await repo.UpdatePlayer(id,newPlayer);
            return result? NoContent() : NotFound();
        }

    }
}



















/*<template>
  <div class="game-container">
    <h1>Game Room</h1>

    <div v-if="gameRoom">
      <h2>Current Question</h2>
      <p>{{ currentQuestion?.text }}</p>

      <input type="text" v-model="answer" placeholder="Your answer" />
      <button @click="submitAnswer">Submit Answer</button>
      <button @click="goToNextQuestion">Next Question</button>
    </div>

    <div v-else>
      <h2>No active game room.</h2>
      <button @click="startNewGame">Start New Game</button>
    </div>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p v-if="gameRoom">Game started successfully: {{ gameRoom }}</p>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      adminId: 1, // Replace with the actual admin ID
      playerIds: [1, 2], // Replace with actual player IDs
      gameRoom: null, // To store game room details
      currentQuestion: null, // To store the current question details
      answer: '', // To store player's answer
      errorMessage: '', // To display error messages
    };
  },
  methods: {
    async startNewGame() {
      const requestBody = {
        AdminId: this.adminId,
        PlayerIds: this.playerIds,
      };

      try {
        const response = await axios.post('http://localhost:5180/Backend/Game/Start', requestBody);
        this.gameRoom = response.data; // Store the game room details
        await this.fetchCurrentQuestion(); // Call this method to fetch the current question
      } catch (error) {
        // Set error message based on the response, if available
        this.errorMessage = error.response?.data || 'Failed to start a new game.';
        console.error('Error starting game:', this.errorMessage); // Log the error to the console
      }
    },
    async fetchCurrentQuestion() {
      if (!this.gameRoom) return; // Ensure gameRoom is available before fetching

      try {
        const response = await axios.get(`http://localhost:5180/Backend/Game/${this.gameRoom.roomCode}/CurrentQuestion`);
        this.currentQuestion = response.data; // Store the current question
        console.log('Current question:', response.data); // Log the current question for debugging
      } catch (error) {
        console.error('Error fetching current question:', error.response?.data || error.message);
        this.errorMessage = error.response?.data || 'Failed to fetch current question.'; // Set error message
      }
    },
    async submitAnswer() {
      if (!this.answer) {
        this.errorMessage = 'Please enter an answer.';
        return;
      }

      const requestBody = {
        PlayerId: this.adminId, // Change this to the actual player ID as needed
        Answer: this.answer,
      };

      try {
        const response = await axios.post(`http://localhost:5180/Backend/Game/${this.gameRoom.roomCode}/Answer`, requestBody);
        const isCorrect = response.data.IsCorrect;
        alert(isCorrect ? 'Correct answer!' : 'Wrong answer!');
        this.answer = ''; // Clear the input field after submission
      } catch (error) {
        this.errorMessage = error.response?.data || 'Failed to submit answer.';
        console.error('Error submitting answer:', this.errorMessage); // Log error for debugging
      }
    },
    async goToNextQuestion() {
      try {
        await axios.post(`http://localhost:5180/Backend/Game/${this.gameRoom.roomCode}/NextQuestion`);
        await this.fetchCurrentQuestion(); // Fetch the next question after successful navigation
      } catch (error) {
        this.errorMessage = error.response?.data || 'Failed to move to the next question.';
        console.error('Error moving to the next question:', this.errorMessage); // Log error for debugging
      }
    },
  },
};
</script>

<style scoped>
.game-container {
  max-width: 600px;
  margin: auto;
  padding: 20px;
  border: 1px solid #ccc;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
}

.error {
  color: red;
}
</style>
*/