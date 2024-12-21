<template>
  <div class="flex-layout">
    <!-- Main Section -->
    <div class="main">
      <div class="button-wrapper">
        <button class="button-start" @click="startGame">Start</button>
        <button
          class="button-secondary"
          :class="{ 'green-button': gameStatus === 1 }"
          @click="openPackages"
        >
          Packages
        </button>
      </div>

      <!-- Room Code Display -->
      <div class="roomCode-display">
        Room Code: <span>{{ roomCode }}</span>
      </div>

      <!-- Players Table -->
      <div class="table-container">
        <table class="users-table">
          <thead>
            <tr>
              <th>User ID</th>
              <th>Username</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in users" :key="user.id">
              <td>{{ user.id }}</td>
              <td>{{ user.username }}</td>
              <td>{{ user.status }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Sidebar Settings -->
    <div class="sidebar">
      <div class="settings-header">Settings</div>

      <div class="settings-section">
        <p>
          Rounds: <strong>{{ rounds }}</strong>
        </p>
        <div class="controls">
          <button @click="decreaseRounds">-</button>
          <button @click="increaseRounds">+</button>
        </div>
      </div>

      <div class="settings-section">
        <p>
          Timer: <strong>{{ roundTime }} sec</strong>
        </p>
        <div class="controls">
          <button @click="decreaseTime">-</button>
          <button @click="increaseTime">+</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import axios from "axios";
import { ref, onMounted, onBeforeUnmount } from "vue";
import { useUserStore } from "@/stores/userStore";
import axiosInstance, { isTokenExpired } from "~/utils/axiosInstance";
import router from "~/itb2203-chatapp/src/router";

const userStore = useUserStore();
// Define interfaces for the data
interface User {
  id: number;
  username: string;
  status: string;
}
const fetchLoggedInUserGameRoomId = async (): Promise<number | null> => {
  try {
    const userId = userStore.userId;
    console.log(userId);
    const response = await axios.get(
      `http://localhost:5180/Backend/Lobby/Player/${userId}`,
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    const roomId = response.data; // Assuming response.data is directly the room ID
    return roomId;
  } catch (error) {
    console.error("Error fetching game room ID:", error);
    return null;
  }
};
// State variables
const users = ref<User[]>([]);
let gameStatusInterval = ref<null | number>(null);
let roomCode = ref<string | null>(null);
const roomId = ref<number | null>(null);
const rounds = ref<number>(3);
const roundTime = ref<number>(60);
const selectedPackage = ref<null | string>(null);
const pollInterval = ref<null | number>(null);
const loading = ref<boolean>(false);
const groupCount = ref<number>(2);
let gameStatus = ref<number | null>(null);

// Fetch all players initially
const fetchUsers = async (): Promise<void> => {
  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Lobby/${roomId.value}`,
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    roomCode = response.data.roomCode;
    const newPlayers: User[] = response.data.players || [];

    const currentIds = new Set(users.value.map((user) => user.id));
    newPlayers.forEach((player) => {
      if (!currentIds.has(player.id)) {
        users.value.push({
          id: player.id,
          username: player.username,
          status: player.status || "Active",
        });
      }
    });
  } catch (error) {
    console.error("Error polling for new players:", error);
  }
};

// Poll for new players
const pollNewPlayers = async (): Promise<void> => {
  try {
    const response = await axios.get(
      `https://localhost:7269/Backend/Lobby/${roomId.value}`
    );
    const newPlayers: User[] = response.data.players || [];

    const currentIds = new Set(users.value.map((user) => user.id));
    newPlayers.forEach((player) => {
      if (!currentIds.has(player.id)) {
        users.value.push({
          id: player.id,
          username: player.username,
          status: player.status || "Active",
        });
      }
    });
  } catch (error) {
    console.error("Error polling for new players:", error);
  }
};
const fetchGameStatus = async (): Promise<void> => {
  try {
    if (!roomId.value) {
      console.warn("Room ID is not available.");
      return;
    }

    const response = await axios.get(
      `http://localhost:5180/Backend/Lobby/${roomId.value}/status`
    );

    gameStatus.value = response.data.status;
    console.log("Fetched game status:", response.data);

    if (response.data === 1) {
      const redirectUrl = `/Game/Answering`;
      console.log("Redirecting to:", redirectUrl);

      window.location.href = redirectUrl;
    }
  } catch (error) {
    console.error("Error fetching game status:", error);
  }
};

onMounted(async () => {
  if (typeof window !== "undefined") {
    // Fetch the logged-in user's game room ID first
    const gameRoomId = await fetchLoggedInUserGameRoomId();

    // Once the game room ID is fetched, fetch the users
    if (gameRoomId !== null) {
      roomId.value = gameRoomId; // Update the roomId state with the fetched value
      fetchUsers(); // Now fetch the users for that room
    }

    // Start polling for new players
    pollInterval.value = window.setInterval(pollNewPlayers, 2000);
    gameStatusInterval.value = window.setInterval(fetchGameStatus, 4000);
  }
});

onBeforeUnmount(() => {
  if (pollInterval.value !== null) {
    clearInterval(pollInterval.value);
  }

  if (gameStatusInterval.value !== null) {
    clearInterval(gameStatusInterval.value);
  }
});

const startGame = async (): Promise<void> => {
  const userStore = useUserStore();

  try {
    if (!userStore.token || isTokenExpired(userStore.tokenExpiration)) {
      alert("Your session has expired. Please log in again.");
      userStore.logOut();
      window.location.href = "/"; // Redirect to login
      return;
    }

    loading.value = true;

    const requestBody = {
      players: users.value,
      rounds: rounds.value,
      timerForAnsweringInSec: roundTime.value,
      playersPerGroup: groupCount.value,
    };
    alert(`request data : ${JSON.stringify(users.value)}, ${rounds.value}, ${roundTime.value}, ${groupCount.value}`)
    console.log("Request body for creating the game:", JSON.stringify(requestBody,null,2));

    const response = await axiosInstance.post("Game/create", requestBody);

    console.log("Game creation response:", response.data);

    // const firstRound = response.data.gameRounds[0];
    // const firstGroup = firstRound.groups[0];

    // if (!firstRound || !firstGroup) {
    //   console.error("No round or group data available for redirection.");
    //   return;
    // }

    //change gameStatus to "1"
    await axios.put(
      `http://localhost:5180/Backend/Lobby/${roomId.value}/status/1`
    );
  } catch (error) {
    console.error("Error starting the game:", error);
    alert("Failed to start the game. Please try again.");
  } finally {
    loading.value = false;
  }
};

const openPackages = (): void => {
  console.log("Opening Packages...");
};

const increaseRounds = (): void => {
  rounds.value += 1;
};

const decreaseRounds = (): void => {
  if (rounds.value > 1) rounds.value -= 1;
};

const increaseTime = (): void => {
  roundTime.value += 10;
};

const decreaseTime = (): void => {
  if (roundTime.value > 10) roundTime.value -= 10;
};

const increaseGroups = (): void => {
  groupCount.value += 1;
};

const decreaseGroups = (): void => {
  if (groupCount.value > 2) {
    groupCount.value -= 1;
  }
};
</script>

<style scoped>
.green-button {
  background-color: #27ae60;
  color: white;
}
.flex-layout {
  display: flex;
  height: 100vh;
  background: linear-gradient(135deg, #f4f4ff, #e6d8f7);
  gap: 10px;
  padding: 20px;
  box-sizing: border-box;
}

.main {
  flex: 3;
  background: #ffffff;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  padding: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.sidebar {
  flex: 1;
  background: #7d3c98;
  color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.settings-header {
  font-size: 1.5rem;
  font-weight: bold;
  text-align: center;
  margin-bottom: 10px;
}

.settings-section {
  background: #ffffff33;
  padding: 10px;
  border-radius: 8px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.button-wrapper {
  display: flex;
  gap: 20px;
  margin-bottom: 30px;
}

button {
  background-color: #8e44ad;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s;
}

button:hover {
  background-color: #732d91;
  transform: scale(1.05);
}

.button-start {
  background-color: #27ae60;
}

.button-secondary {
  background-color: #f39c12;
}

.table-container {
  margin-top: 20px;
  overflow-x: auto;
  width: 100%;
}

.users-table {
  width: 100%;
  border-collapse: collapse;
  border-radius: 10px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.users-table th {
  background-color: #8e44ad;
  color: white;
  padding: 10px;
  text-align: left;
}

.users-table td {
  padding: 10px;
  border-bottom: 1px solid #ddd;
}

.users-table tr:nth-child(even) {
  background-color: #f4f4f9;
}

.roomCode-display {
  margin-top: 10px;
  font-size: 1.2rem;
  color: #5b2c6f;
  background: rgba(245, 245, 245, 0.8);
  padding: 10px 15px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.controls button {
  width: 40px;
  height: 40px;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 50%;
}
</style> 
