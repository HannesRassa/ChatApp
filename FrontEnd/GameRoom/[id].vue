<template>
    <div class="container">
      <h1>Game Rooms</h1>
      <div class="actions">
        <button @click="createGameRoom">Create Game Room</button>
      </div>
      <table class="gamerooms-table">
        <thead>
          <tr>
            <th>Room ID</th>
            <th>Room Code</th>
            <th>Admin Username</th>
            <th>Players</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="room in gameRooms" :key="room.id">
            <td>{{ room.id }}</td>
            <td>{{ room.roomCode }}</td>
            <td>{{ room.admin.username }}</td>
            <td>{{ room.players.length }}</td>
            <td>
              <button @click="joinGameRoom(room.roomCode)">Join Room</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="error">{{ error }}</p>
    </div>
  </template>
  
  <script>
  import { ref, onMounted } from 'vue';
  import { useRouter } from 'vue-router';
  
  export default {
    setup() {
      const gameRooms = ref([]);
      const error = ref(null);
      const router = useRouter();
  
      // Fetch all game rooms
      const fetchGameRooms = async () => {
        try {
          const response = await fetch('http://localhost:5180/backend/gameroom');
          if (!response.ok) throw new Error("Failed to fetch game rooms.");
          gameRooms.value = await response.json();
        } catch (err) {
          error.value = err.message;
        }
      };
  
      // Create a new game room
      const createGameRoom = async () => {
        const adminId = prompt("Enter your Player ID to create a game room:");
        if (adminId) {
          try {
            const response = await fetch('http://localhost:5180/backend/gameroom', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
              },
              body: JSON.stringify(adminId),
            });
            if (!response.ok) throw new Error("Failed to create game room.");
            await fetchGameRooms(); // Refresh the list after creation
          } catch (err) {
            error.value = err.message;
          }
        }
      };
  
      // Join an existing game room
      const joinGameRoom = async (roomCode) => {
        const playerId = prompt("Enter your Player ID to join the game room:");
        if (playerId) {
          try {
            const response = await fetch('http://localhost:5180/backend/gameroom/joinroom', {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
              },
              body: JSON.stringify({ playerId: Number(playerId), roomCode: roomCode }),
            });
            if (!response.ok) throw new Error("Failed to join game room.");
            alert("Successfully joined the game room!");
            await fetchGameRooms(); // Refresh the list after joining
          } catch (err) {
            error.value = err.message;
          }
        }
      };
  
      // Fetch game rooms on component mount
      onMounted(fetchGameRooms);
  
      return {
        gameRooms,
        error,
        createGameRoom,
        joinGameRoom,
      };
    },
  };
  </script>
  
  <style scoped>
  .container {
    padding: 20px;
  }
  
  .actions {
    margin-bottom: 20px;
  }
  
  .gamerooms-table {
    width: 100%;
    border-collapse: collapse;
  }
  
  .gamerooms-table th,
  .gamerooms-table td {
    border: 1px solid #ddd;
    padding: 10px;
  }
  
  .gamerooms-table th {
    background-color: #007bff;
    color: white;
  }
  
  button {
    background-color: #007bff;
    color: white;
    padding: 10px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  </style>
  