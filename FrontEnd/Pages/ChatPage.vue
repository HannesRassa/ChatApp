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
          <button @click="joinGame(game)">Join Game</button>
        </li>
      </ul>
    </div>
    <p v-else>No games available.</p>
    <p v-if="error">{{ error }}</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';

const games = ref([]);
const error = ref(null);
const router = useRouter();

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

// Navigate to game details page
const joinGame = (game) => {
  router.push(`/Game/${game.id}`);
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
