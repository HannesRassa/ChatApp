<template>
  <div>
    <h1>Add Player Points</h1>
    <!-- <strong>Current Points:</strong> {{ currentPlayer.points || 0 }}</p> -->
    <form @submit.prevent="submitPlayerPoint">
      <div>
        <label for="playerId">Player ID:</label>
        <input
          type="number"
          v-model="playerPoint.playerId"
          id="playerId"
          required
        />
      </div>
      <div>
        <div>
          <label for="gameId">Game ID:</label>
          <input
            type="number"
            v-model="playerPoint.gameId"
            id="playerId"
            required
          />
        </div>
        <label for="points">Points:</label>
        <input
          type="number"
          v-model="playerPoint.points"
          id="points"
          required
        />
      </div>
      <button type="submit">Submit</button>
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
  const userId = userStore.userId;
  try {
    const response = await axios.post(
      "http://localhost:5180/Backend/PlayerPoint",
      {
        playerId: playerPoint.value.playerId,
        gameId: playerPoint.value.gameId,
        points: playerPoint.value.points,
      }
    );
    console.log(`${playerPoint.value.playerId}`);
    console.log("Response data:", response.data.value); // Log the response data on success
    alert("Player point added successfully!");
  } catch (error: any) {
    // Log error details
    if (error.response) {
      // The request was made and the server responded with a status code outside of the 2xx range
      console.error("Error response data:", error.response.data);
      console.error("Error response status:", error.response.status);
    } else if (error.request) {
      // The request was made but no response was received
      console.error("Error request:", error.request);
    } else {
      // Something else went wrong in setting up the request
      console.error("Error message:", error.message);
    }
    alert("Failed to add player point.");
  }
};
const fetchPlayerById = async () => {
  const userId = userStore.userId;
  const response = await axios.get(
    `http://localhost:5180/Backend/Player/${userId}`
  );
  currentPlayer = response.data;
};
onMounted(() => {
  userStore.loadUser();
  if (!userStore.userId) {
    console.error("User ID is not set. Ensure the user is logged in.");
  }
  fetchPlayerById();
});
</script>

<style>
form {
  display: flex;
  flex-direction: column;
  gap: 10px;
  max-width: 300px;
}
button {
  margin-top: 10px;
}
</style>
