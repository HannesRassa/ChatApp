<template>
    <div>
      <h1>Game Management</h1>
      <div v-if="games.length">
        <h2>Available Games</h2>
        <ul>
          <li v-for="game in games" :key="game.id">
            <strong>Game ID:</strong> {{ game.id }}<br>
            <strong>Rounds:</strong> {{ game.rounds }}<br>
            <strong>Timer:</strong> {{ game.timerForAnsweringInSec }} seconds<br>
            <strong>Players per Group:</strong> {{ game.playersPerGroup }}<br>
            <button @click="JoinGame(game)">Join Game</button>
          </li>
        </ul>
      </div>
      <p v-else>No games available.</p>
  
      <div v-if="isCountdownActive">
        <h2>Countdown: {{ countdown }}</h2>
      </div>
  
      <div v-if="gameInProgress">
        <h2>The game has started!</h2>
        <!-- Additional game logic goes here -->
      </div>
  
      <p v-if="error">{{ error }}</p>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  
  const games = ref([]);
  const countdown = ref(0);
  const isCountdownActive = ref(false);
  const gameInProgress = ref(false);
  const error = ref(null);
  
  // Fetch games from the API
  const fetchGames = async () => {
    try {
      const response = await fetch("http://localhost:5180/backend/game");
      if (!response.ok) throw new Error("Failed to fetch games.");
      games.value = await response.json();
    } catch (err) {
      error.value = err.message;
    }
  };
  
  // Start the game with a countdown
  const JoinGame = (game) => {
    // Set the countdown based on the game's timer
    countdown.value = game.timerForAnsweringInSec;
    isCountdownActive.value = true;
    
    const countdownInterval = setInterval(() => {
      if (countdown.value > 0) {
        countdown.value--;
      } else {
        clearInterval(countdownInterval);
        isCountdownActive.value = false;
        gameInProgress.value = true; // Start the game after countdown
      }
    }, 1000);
  };
  
  // Fetch games when the component is mounted
  onMounted(fetchGames);
  </script>
  
  <style scoped>
  ul {
    list-style-type: disc;
    padding-left: 20px;
  }
  
  li {
    margin-bottom: 16px;
  }
  
  button {
    margin-top: 8px;
  }
  </style>
  