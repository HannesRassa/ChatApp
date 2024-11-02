<template>
  <div class="main-menu">
    <h1>Main Menu</h1>
    <div class="menu-options">
      <button @click="startNewGame">Start New Game</button>
      <button @click="searchForGames">Search for Existing Games</button>
      <button @click="createQuestion">Create Question</button>
      <button @click="browseQuestions">Browse Questions</button>
      <li><NuxtLink to="/">Home</NuxtLink></li>
    </div>
  </div>
</template>
<script lang="ts">
import { useRouter } from 'vue-router';
import axios, { AxiosError } from 'axios';
import { useUserStore } from '@/stores/userStore'; // adjust the path as needed
import { defineComponent } from 'vue';

export default defineComponent({
  setup() {
    const router = useRouter();
    const userStore = useUserStore();

    const startNewGame = async (): Promise<void> => {
      try {
        const userId = userStore.userId;
        if (!userId) {
          console.error("User is not logged in");
          return;
        }


        // Make the POST request with the adminId in the body
        const response = await axios.post(
          'http://localhost:5180/Backend/GameRoom?Id=', 
          { adminId: userId }
        );

        if (response.data) {
          const roomCode = response.data.roomCode;
          console.log("Game room created with room code:", roomCode);
          await router.push({ name: 'LobbyPage'});
        }
      } catch (error) {
        // Handle AxiosError separately for better error reporting
        if (axios.isAxiosError(error)) {
          const axiosError = error as AxiosError;
          console.error("Error creating game room:", axiosError.message);
        } else {
          console.error("Unexpected error:", error);
        }
      }
    };
    const searchForGames = () => {
      console.log("Searching for existing games...");
    };

    const createQuestion = () => {
      console.log("Creating a new question...");
    };

    const browseQuestions = () => {
      console.log("Browsing questions...");
      router.push({ name: 'BrowseQuestions' });
    };

    return {
      startNewGame,
      searchForGames,
      createQuestion,
      browseQuestions,
    };




}});

</script>
<style scoped>
.main-menu {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
  text-align: center;
  padding: 20px;
}

.menu-options {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

button {
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s;
}

button:hover {
  background-color: #f0f0f0;
}
</style>