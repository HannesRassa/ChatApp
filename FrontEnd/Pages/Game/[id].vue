<template>
  <div class="game-container">
    <!-- Game ID -->
    <div class="game-id">
      <p>Game ID: {{ gameData.id }}</p> <!-- Access id without .value -->
    </div>

    <!-- Question Text -->
    <div class="question-box">
      <p>{{ gameData.questionText }}</p> <!-- Access questionText without .value -->
    </div>

    <!-- Opponent Section -->
    <div class="opponent">
      <p>Opponent</p>
      <div class="opponent-icon">✓</div> <!-- placeholder icon -->
    </div>

    <!-- Answer Box -->
    <div class="answer-box">
      <input
        v-model="answer"
        placeholder="Type your answer here"
        class="answer-input"
      />
    </div>

    <!-- Player Points -->
    <div class="points">
      <p>Points: {{ gameData.id }}</p> <!-- Access points without .value -->
    </div>
  </div>
</template>

<script setup>
import { useRoute } from 'vue-router'; // Import useRoute
import { ref, onMounted } from 'vue'; // Import ref and onMounted
import axios from 'axios';

const route = useRoute(); // Access route parameters
const gameData = ref({
  id: '',
  questionText: '',
  player: {
    points: 0,
  },
});
const answer = ref(''); // Use ref for the answer

// Fetch game data based on the ID from the route parameters
const fetchGameData = async () => {
  const { id } = route.params; // Destructure the ID from route parameters
  console.log('Fetching data for Game ID:', id); // Debugging: Log the Game ID
  try {
    const response = await axios.get(`http://localhost:5180/Backend/Game/${id}`); // Use template literals
    console.log('Response data:', response.data); // Debugging: Log the response data
    gameData.value = response.data; 
  } catch (error) {
    console.error('Error fetching game data:', error);
  }
};

onMounted(() => {
  fetchGameData();
});
</script>

<style scoped>
/* Your existing styles here */
</style>
