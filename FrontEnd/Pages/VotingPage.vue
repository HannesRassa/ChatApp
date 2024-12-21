<template>
  <div class="form-container">
    <h1 class="form-title">Add Player Points</h1>

    <form @submit.prevent="submitPlayerPoint" class="form">
      <div class="form-group">
        <label for="playerId" class="form-label">Player ID:</label>
        <input
          type="number"
          v-model="playerPoint.playerId"
          id="playerId"
          class="form-input"
          required
        />
      </div>

      <div class="form-group">
        <label for="gameId" class="form-label">Game ID:</label>
        <input
          type="number"
          v-model="playerPoint.gameId"
          id="gameId"
          class="form-input"
          required
        />
      </div>

      <div class="form-group">
        <label for="points" class="form-label">Points:</label>
        <input
          type="number"
          v-model="playerPoint.points"
          id="points"
          class="form-input"
          required
        />
      </div>

      <button type="submit" class="form-button">Submit</button>
    </form>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from "vue";
import { useUserStore } from "@/stores/userStore";
import axios from "axios";

const userStore = useUserStore();
let currentPlayer = ref({
  userName: "string",
  password: "passString",
});
const playerPoint = ref({
  playerId: 0,
  gameId: 0,
  points: 0,
});

const submitPlayerPoint = async () => {
  try {
    const response = await axios.post("https://localhost:7269/Backend/PlayerPoint", {
      playerId: playerPoint.value.playerId,
      gameId: playerPoint.value.gameId,
      points: playerPoint.value.points,
    });
    console.log("Response data:", response.data);
    alert("Player point added successfully!");
  } catch (error: any) {
    console.error("Error submitting player points:", error);
    alert("Failed to add player point.");
  }
};

const fetchPlayerById = async () => {
  const userId = userStore.userId;
  try {
    const response = await axios.get(`https://localhost:7269/Backend/Player/${userId}`);
    currentPlayer.value = response.data;
  } catch (error) {
    console.error("Error fetching player data:", error);
  }
};

onMounted(() => {
  userStore.loadUser();
  fetchPlayerById();
});
</script>

<style scoped>
.form-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
  background: linear-gradient(135deg, #f4f4ff, #e6d8f7);
  padding: 20px;
}

.form-title {
  font-size: 2rem;
  font-weight: bold;
  color: #5b2c6f;
  margin-bottom: 20px;
}

.form {
  background: #ffffff;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.form-label {
  font-size: 1rem;
  font-weight: bold;
  color: #7d3c98;
}

.form-input {
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.form-input:focus {
  outline: none;
  border-color: #8e44ad;
  box-shadow: 0 0 4px rgba(142, 68, 173, 0.5);
}

.form-button {
  background-color: #8e44ad;
  color: white;
  font-size: 1rem;
  padding: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s, transform 0.2s;
}

.form-button:hover {
  background-color: #732d91;
  transform: scale(1.05);
}
</style>
