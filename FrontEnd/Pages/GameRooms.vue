<template>
  <div class="flex-layout">
    <div class="main">
      <!-- RoomCode Display -->
      <div class="roomCode-display">
          {{ roomCode }}
      </div>

      <!-- Game Rooms Table -->
      <table class="users-table">
        <thead>
          <tr>
            <th>Lobby ID</th>
            <th>RoomCode</th>
            <th>Connect</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="gameRoom in gameRooms" :key="gameRoom.id">
            <td>{{ gameRoom.id }}</td>
            <td>{{ gameRoom.RoomCode }}</td>
            <td><button @click="connectToGame(gameRoom.RoomCode)">Connect</button></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
<template>
  <div class="flex-layout">
    <div class="main">
      <table class="users-table">
        <thead>
          <tr>
            <th>Lobby ID</th>
            <th>RoomCode</th>
            <th>Connect</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="gameRoom in gameRooms" :key="gameRoom.id">
            <td>{{ gameRoom.id }}</td>
            <td>{{ gameRoom.RoomCode }}</td>
            <td><button @click="connectToLobby(gameRoom.id)">Connect</button></td>
          </tr>
        </tbody>
      </table>
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
    connectToLobby(roomCode) {
      // Handle the logic to connect to the selected game room
      console.log("Connecting to room: ", roomCode);
      // Redirect or perform any action needed to connect to the game
      // Example: this.$router.push(`/game/${roomCode}`)
    }
  }
}
</script>

<style scoped>
  .flex-layout {
    display: flex;
    height: 100vh; /* Full viewport height */
    padding: 20px;
    background-color: #19222d; /* Light lavender background for the layout */
  }

  .main {
    flex: 4; /* Takes up 4/6 of the available width (more space than the sidebar) */
    padding: 20px;
    text-align: center;
    background-color: #ececec; 
    display: flex;
    flex-direction: column;
    justify-content: flex-start; 
    align-items: center; 
    border-top-left-radius: 10px;
    border-bottom-left-radius: 10px;
  }

  .roomCode-display {
    position: absolute;
    top: 70px;
    left: 50px;
    font-size: 16px;
    color: #f9f9f9;
    font-weight: bold;
    background-color: rgba(245, 245, 245, 0.8); 
    padding: 5px 10px;
    border-radius: 5px;
  }

  button {
    background-color: #bfbfbf; 
    color: black;
    padding: 10px 20px;
    border-radius: 8px;
    font-size: 16px;
    cursor: pointer;
    border: none;
    transition: background-color 0.3s;
  } 

  button:hover {
    background-color: #a29b97; 
  }
  
  .users-table {
    background-color: #bfbfbf; 
    color: black;
    width: 0%;
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
    background-color: #d1d1d1; 
  }

  .users-table tr:nth-child(even) {
    background-color: #dfdfdf; 
  }

  .users-table tr:nth-child(odd) {
    background-color: #d7d7d7; 
  }
</style>

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
      // Handle the logic to connect to the selected game room
      console.log("Connecting to room: ", roomCode);
      // Redirect or perform any action needed to connect to the game
      // Example: this.$router.push(`/game/${roomCode}`)
    }
  }
}
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
    padding: 10px 20px;
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
</style>
