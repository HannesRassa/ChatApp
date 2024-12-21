<template>
  <div class="flex-layout">
    <div class="main">
      <h1 class="section-title">Available Game Rooms</h1>
      <div class="table-container">
        <table class="users-table">
          <thead>
            <tr>
              <th>Lobby ID</th>
              <th>Room Code</th>
              <th>Connect</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="gameRoom in gameRooms" :key="gameRoom.id">
              <td>{{ gameRoom.id }}</td>
              <td>{{ gameRoom.RoomCode }}</td>
              <td>
                <button class="connect-button" @click="connectToGame(gameRoom.RoomCode)">Connect</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      roomCode: "Lobby",
      gameRooms: [
        { id: 1, RoomCode: "ROOM001" },
        { id: 2, RoomCode: "ROOM002" },
        { id: 3, RoomCode: "ROOM003" }
      ]
    };
  },
  methods: {
    connectToGame(roomCode) {
      console.log("Connecting to room: ", roomCode);
      this.$router.push(`LobbyPage`);
    }
  }
};
</script>

<style scoped>
.flex-layout {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: linear-gradient(135deg, #2c3e50, #4ca1af);
  padding: 20px;
  box-sizing: border-box;
}

.main {
  width: 80%;
  background-color: #ffffff;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  padding: 20px;
  text-align: center;
  animation: fadeIn 0.5s ease-in-out;
}

.section-title {
  font-size: 2rem;
  color: #34495e;
  margin-bottom: 20px;
  font-weight: bold;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
}

.table-container {
  overflow-x: auto;
  margin-top: 10px;
}

.users-table {
  width: 100%;
  border-collapse: collapse;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.users-table th, .users-table td {
  padding: 12px;
  text-align: center;
  color: #000000; 
}

.users-table th {
  background-color: #4ca1af;
  color: #ffffff;
  text-transform: uppercase;
  font-size: 0.9rem;
}

.users-table tr:nth-child(even) {
  background-color: #f9f9f9;
}

.users-table tr:nth-child(odd) {
  background-color: #f1f1f1;
}

.connect-button {
  background-color: #2ecc71;
  color: #000000;
  border: none;
  padding: 8px 16px;
  font-size: 1rem;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.connect-button:hover {
  background-color: #27ae60;
  transform: translateY(-2px);
}

/* Fade-in animation */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
