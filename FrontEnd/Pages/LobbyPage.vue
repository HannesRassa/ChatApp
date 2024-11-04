<template>
  <div class="container">
    <div class="main">
      <button @click="startGame">Start</button>
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
    <div class="sidebar">
      <div class="game-info">
        <p>Rounds: {{ rounds }}</p>
      </div>
    </div>
  </div>
</template>

<script>
import { useRouter } from 'vue-router'; // Import useRouter

export default {
  data() {
    return {
      users: [
        { id: 1, username: "PlayerOne", status: "Ready" },
        { id: 2, username: "PlayerTwo", status: "Waiting" }
      ],
      rounds: 1
    };
  },
  setup() {
    const router = useRouter(); // Access the router instance

    // Define the startGame method
    const startGame = () => {
      console.log("Game Started!");
      // Redirect to the game page
      router.push('Game/1'); // Replace with your actual path
    };

    return {
      startGame // Return the startGame method to be used in the template
    };
  }
};
</script>

<style scoped>
  .container {
  display: flex;
  flex-direction: row;
  justify-content: flex-start; 
  align-items: flex-start;
  height: 100vh;
  background-color: #f4f4f4;
  width: 100%; 
  
  }


.main {
  flex: 3; 
  padding: 20px;
  text-align: center;
}
.sidebar {
  flex: 1; 
  background-color: #f1f1f1;
  padding: 20px;
  border-left: 1px solid #ddd;
}
.game-info {
  background-color: #007bff;
  color: white;
  padding: 10px;
  border-radius: 5px;
  text-align: center;
  font-size: 20px;
}
button {
  background-color: #007bff;
  color: white;
  padding: 10px 20px;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  margin-bottom: 20px; 
}
.users-table {
  background-color: #007bff;
  color: white;
  margin-top: 20px;
  width: 80%;
  border-collapse: collapse;
}
.users-table th,
.users-table td {
  
  padding: 10px;
  border: 1px solid #ddd;
}
.users-table th {
  background-color:  #007bff;
}
</style>
