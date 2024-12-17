<template>
  <div class="flex-layout">
    <div class="main">
      <div class="button-wrapper">
        <button class="custom-style" @click="startGame">Start</button>
        <button class="custom-style" @click="openPackages">Packages</button>
      </div>

      <div class="roomCode-display">
        {{ roomCode }}
      </div>

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

    <div class="sidebar">
      <div class="settings-header">
        <p>Settings</p>
      </div>

      <div class="rounds-settings">
        <p>Rounds: {{ rounds }}</p>
        <div class="rounds-controls">
          <button class="custom-style" @click="decreaseRounds">-</button>
          <button class="custom-style" @click="increaseRounds">+</button>
        </div>
      </div>

      <div class="timer-settings">
        <p>Timer: {{ roundTime }} sec</p>
        <div class="timer-controls">
          <button class="custom-style" @click="decreaseTime">-</button>
          <button class="custom-style" @click="increaseTime">+</button>
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
    console.log(userId)
    const response = await axios.get(`http://localhost:5180/Backend/Lobby/Player/${userId}` ,{
      headers: {
          'Content-Type': 'application/json',
        },
    });
    const roomId = response.data; // Assuming response.data is directly the room ID
    console.log("AAAAAAAAAAAAAAAAAFetched Game Room ID:", roomId);
    return roomId;
  } catch (error) {
    console.error("Error fetching game room ID:", error);
    return null;
  }
};
// State variables
const users = ref<User[]>([]);
const roomCode = ref<string | null>(null);
const roomId = ref<number | null>(null);
const rounds = ref<number>(3);
const roundTime = ref<number>(60);
const selectedPackage = ref<null | string>(null);
const pollInterval = ref<null | number>(null);
const loading = ref<boolean>(false);
const groupCount = ref<number>(2);

// Fetch all players initially
const fetchUsers = async (): Promise<void> => {

  try {
    const response = await axios.get(
      `http://localhost:5180/Backend/Lobby/${roomId.value}`, {
      headers: {
          'Content-Type': 'application/json',
        },
  });
    const newPlayers: User[] = response.data.players || [];
    console.log(
      `fetched response.data: ${JSON.stringify(response.data, null, 2)}`
    );
    console.log(`fetched newPlayers: ${JSON.stringify(newPlayers, null, 2)}`);

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
      `http://localhost:5180/Backend/Lobby/${roomId.value}`
    );
    const newPlayers: User[] = response.data.players || [];
    console.log(
      `fetched response.data: ${JSON.stringify(response.data, null, 2)}`
    );
    console.log(`fetched newPlayers: ${JSON.stringify(newPlayers, null, 2)}`);

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

// Lifecycle hooks
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
    pollInterval.value = window.setInterval(pollNewPlayers, 5000);
  }
});

onBeforeUnmount(() => {
  if (pollInterval.value !== null) {
    clearInterval(pollInterval.value);
  }
});

const startGame = async (): Promise<void> => {
  const userStore = useUserStore();

  try {
    if (!userStore.token || isTokenExpired(userStore.tokenExpiration)) {
      alert('Your session has expired. Please log in again.');
      userStore.logOut();
      window.location.href = '/'; // Redirect to login
      return;
    }

    loading.value = true;

    const requestBody = {
      players: users.value.map((user) => ({
        id: user.id,
        username: user.username
      })),
      rounds: rounds.value,
      timerForAnsweringInSec: roundTime.value,
      playersPerGroup: groupCount.value,
    };

    console.log('Request body for creating the game:', requestBody);

    const response = await axiosInstance.post('Game/create', requestBody);

    console.log('Game creation response:', response.data);

    const firstRound = response.data.gameRounds[0];
    const firstGroup = firstRound.groups[0];

    if (!firstRound || !firstGroup) {
      console.error('No round or group data available for redirection.');
      return;
    }

    const redirectUrl = `/Game/Answering`;
    console.log('Redirecting to:', redirectUrl);

    window.location.href = redirectUrl;
  } catch (error) {
    console.error('Error starting the game:', error);
    alert('Failed to start the game. Please try again.');
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

const dividePlayersIntoGroups = (): User[][] | void => {
  if (users.value.length === 0) {
    console.warn("No players available to divide.");
    return;
  }

  const groups: User[][] = Array.from({ length: groupCount.value }, () => []);
  users.value.forEach((user, index) => {
    groups[index % groupCount.value].push(user);
  });

  console.log("Divided players into groups:", groups);
  return groups; // Optional: return the groups if needed
};
</script>

<style scoped>
.flex-layout {
  display: flex;
  height: 100vh; /* Full viewport height */
  padding: 20px;
  background-color: #f4f0ff; /* Light lavender background for the layout */
}

.main {
  flex: 4; /* Takes up 4/6 of the available width (more space than the sidebar) */
  padding: 20px;
  text-align: center;
  background-color: #e0d4f7; /* Light purple background */
  display: flex;
  flex-direction: column;
  justify-content: flex-start; /* Aligns content to the top */
  align-items: center; /* Centers content horizontally */
  border-top-left-radius: 10px;
  border-bottom-left-radius: 10px;
}

/* Button wrapper */
.button-wrapper {
  display: flex;
  gap: 120px; /* Space between buttons */
  margin-top: 30px; /* Increased space above the buttons */
}

/* RoomCode display */
.roomCode-display {
  position: absolute;
  top: 70px;
  left: 50px;
  font-size: 16px;
  color: #7d3c98;
  font-weight: bold;
  background-color: rgba(255, 255, 255, 0.8); /* Optional background */
  padding: 5px 10px;
  border-radius: 5px;
}

button {
  background-color: #7d3c98; /* Dark purple button */
  color: white;
  padding: 15px 50px;
  border-radius: 8px;
  font-size: 16px;
  cursor: pointer;
  border: none;
  transition: background-color 0.3s;
}

button:hover {
  background-color: #5b2c6f; /* Slightly darker purple on hover */
}

.users-table {
  background-color: #7d3c98; /* Purple table header */
  color: white;
  width: 70%;
  border-collapse: collapse;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #ddd;
  margin-top: 100px; /* Adds space between button and table */
}

.users-table th,
.users-table td {
  padding: 10px;
}

.users-table th {
  background-color: #a569bd; /* Slightly lighter purple for table header */
}

.users-table tr:nth-child(even) {
  background-color: #d3bce7; /* Light purple for even rows */
}

.users-table tr:nth-child(odd) {
  background-color: #c7a6d9; /* Medium purple for odd rows */
}

.sidebar {
  flex: 1; /* Takes up 1/4 of the available width */
  height: 100%; /* Full height of .flex-layout container */
  background-color: #e0d4f7; /* Medium purple background for sidebar */
  padding: 20px;
  border-left: 3px solid #a569bd; /* Purple border to separate sidebar */
  border-top-right-radius: 10px;
  border-bottom-right-radius: 10px;
  display: flex;
  flex-direction: column;
  align-items: flex-start; /* Align content to the top */
  gap: 20px; /* Spacing between each settings section */
}

.settings-header {
  font-size: 22px;
  font-weight: bold;
  color: #5b2c6f;
  width: 100%;
  text-align: center;
  background-color: #a569bd;
  padding: 10px;
  border-radius: 8px;
}

.gameSettings {
  position: absolute;
  top: 10px;
  left: 10px;
  font-size: 16px;
  color: #7d3c98;
  font-weight: bold;
  background-color: rgba(255, 255, 255, 0.8); /* Optional background */
  padding: 5px 10px;
  border-radius: 5px;
}

.package-display {
  background-color: #c7a6d9; /* Darker purple for game info box */
  color: white;
  padding: 10px;
  border-radius: 8px;
  font-size: 20px;
  width: 100%; /* Full width within sidebar */
  display: flex;
  font-weight: bold;
  font-size: 18px;
}

/* Settings sections styling */
.rounds-settings,
.timer-settings {
  display: flex;
  flex-direction: row;
  gap: 50px;
  background-color: #c7a6d9; /* Light purple background */
  padding: 10px;
  border-radius: 8px;
  width: 100%;
}

.rounds-settings p,
.timer-settings p {
  font-size: 18px;
  font-weight: bold;
  color: white;
  margin: 0;
}

.rounds-controls,
.timer-controls {
  display: flex;
  gap: 15px; /* Horizontal space between the buttons */
}

button {
  background-color: #7d3c98;
  color: white;
  width: 30px;
  height: 30px;
  border-radius: 30%;
  font-size: 18px;
  cursor: pointer;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.3s;
}
button.flex-layout {
  background-color: #5b2c6f; /* Slightly darker purple on hover */
}

.rounds-controls button,
.timer-controls button {
  display: flex;
  gap: 10px;
}

.rounds-controls button:hover,
.timer-controls button:hover {
  background-color: #5b2c6f;
}

.timer-settings p {
  font-size: 18px;
  font-weight: bold;
  color: white;
  margin: 0;
}

.group-settings {
  display: flex;
  flex-direction: row;
  gap: 50px;
  background-color: #c7a6d9; /* Light purple background */
  padding: 10px;
  border-radius: 8px;
  width: 100%;
}

.group-settings p {
  font-size: 18px;
  font-weight: bold;
  color: white;
  margin: 0;
}

.group-controls {
  display: flex;
  gap: 15px; /* Horizontal space between the buttons */
}
</style>
