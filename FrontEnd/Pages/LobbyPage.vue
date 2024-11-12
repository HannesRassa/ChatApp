<template>
  <div class="flex-layout">
    <div class="main">
      <!-- Button Wrapper -->
      <div class="button-wrapper">         
        <button @click="startGame">Start</button>
        <button @click="openPackages">Packages</button>
      </div>
      <!-- Nickname Display -->
      <div class="roomCode-display">
          {{ roomCode }}
      </div>

      <!-- Users Table -->
      <table class="users-table">
        <thead>
          <tr>
            <th>User ID</th>
            <th>Username</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(user, index) in users" :key="index">
            <td>{{ user.id }}</td>
            <td>{{ user.username }}</td>
            <td>{{ user.status }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Sidebar -->
    <div class="sidebar">
      <!-- Settings Header -->
      <div class="settings-header">
        <p>Settings</p>
      </div>


      <!-- Rounds Settings -->
      <div class="rounds-settings">
        <p>Rounds: {{ rounds }}</p>
        <div class="rounds-controls">
          <button2 @click="decreaseRounds">-</button2>
          <button2 @click="increaseRounds">+</button2>
        </div>
      </div>

      <!-- Timer Settings -->
      <div class="timer-settings">
        <p>Timer: {{ roundTime }} sec</p>
        <div class="timer-controls">
          <button2 @click="decreaseTime">-</button2>
          <button2 @click="increaseTime">+</button2>
        </div>
      </div>

      <!-- Package Display -->
      <div class="package-display">
        <p>Package: {{ selectedPackage }}</p>
      </div>

    </div>

  </div>
</template>

<script>
export default {
  data() {
    return {
      users: [
        { id: 1, username: "PlayerOne", status: "Ready" },
        { id: 2, username: "PlayerTwo", status: "Waiting" }
      ],
      rounds: 1,
      roomCode: "RoomCode: xxxx",  // Set this to the personal nickname
      roundTime: 60,  // Default round time in seconds
      selectedPackage: "Default Package"  // Default package nam
    };
  },
  methods: {
    startGame() {
      console.log("Game Started!");
    },
    openPackages() {
      console.log("Opening Packages...");
    },
    increaseRounds() {
      this.rounds += 1;
    },
    decreaseRounds() {
      if (this.rounds > 1) this.rounds -= 1;
    },
    increaseTime() {
      this.roundTime += 10;
    },
    decreaseTime() {
      if (this.roundTime > 10) this.roundTime -= 10;
    }
  }
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

  /* Nickname display */
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

  .gameSettings{
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
  .rounds-settings, .timer-settings {
    display: flex;
    flex-direction: row;
    gap: 50px;
    background-color: #c7a6d9; /* Light purple background */
    padding: 10px;
    border-radius: 8px;
    width: 100%;
    
  }

  .rounds-settings p, .timer-settings p {
    font-size: 18px;
    font-weight: bold;
    color: white;
    margin: 0;
  }

  .rounds-controls, .timer-controls {
    display: flex;
    gap: 15px; /* Horizontal space between the buttons */
  }

  button2 {
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
  button2.flex-layout {
    background-color: #5b2c6f; /* Slightly darker purple on hover */
  }

  .rounds-controls button2, .timer-controls button2 {
    display: flex;
    gap: 10px;
  }

  .rounds-controls button2:hover, .timer-controls button2:hover {
    background-color: #5b2c6f;
  }  

  .timer-settings p {
    font-size: 18px;
    font-weight: bold;
    color: white;
    margin: 0;
  }


</style>
