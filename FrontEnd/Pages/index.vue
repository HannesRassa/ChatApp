<template>
  <div class="main-menu">
    <div class="top-right">
      <div v-if="!loggedIn">
        <button @click="toggleSignUpMode">Sign Up</button>
        <button @click="toggleLoginMode">Log In</button>
      </div>
      <div v-else>
        <span @click="toggleDropdown" class="username">{{ username }}</span>
        <div v-if="showDropdown" class="dropdown-menu">
          <button @click="logOut">Log Out</button>
        </div>
      </div>
    </div>


    <div v-if="signUpMode || loginMode" class="form-container">
      <h2>{{ signUpMode ? "Sign Up" : "Log In" }}</h2>
      <form @submit.prevent="signUpMode ? signUp : logIn">
        <div>
          <label for="username">Username:</label>
          <input type="text" id="username" v-model="usernameInput" required />
        </div>
        <div>
          <label for="password">Password:</label>
          <input type="password" id="password" v-model="passwordInput" required />
        </div>
        <button v-if="signUpMode" @click="signUp">Sign Up</button>
        <button v-if="loginMode" @click="logIn">Log In</button>
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
      </form>
    </div>

    <div v-if="loggedIn" class="menu-options">
      <button @click="startNewGame">Start New Game</button>
      <button @click="searchForGames">Search for Existing Games</button>
      <button @click="toggleQuestionDropdown">Create Question</button>
      <button @click="browseQuestions">Browse Questions</button>
    </div>

    <div v-if="isDropdownOpen" class="question-form">
      <form @submit.prevent="submitQuestion">
        <label>
          Question Text:
          <input v-model="questionText" type="text" required />
        </label>
        <label>
          Answer Text:
          <input v-model="answerText" type="text" required />
        </label>
        <label>
          Points:
          <input v-model.number="answerPoints" type="number" min="0" required />
        </label>
        <button type="submit">Submit Question</button>
      </form>
    </div>
    <div v-if="showRoomCodeDropdown" class="room-code-dropdown">
      <label for="roomCode">Enter Room Code:</label>
      <input type="number" id="roomCode" v-model="roomCodeInput" placeholder="e.g., 1234" required />
      <button @click="joinRoom">Join Room</button>
      <p v-if="joinRoomError" class="error">{{ joinRoomError }}</p>
    </div>
  </div>
</template>


<script lang="ts">
import axios from 'axios';
import { defineComponent } from 'vue';
import { useUserStore } from '@/stores/userStore';

export default defineComponent({
  data() {
    return {
      usernameInput: '',
      passwordInput: '',
      username: '',
      loggedIn: false,
      signUpMode: false,
      loginMode: false,
      errorMessage: '',
      userId: null,
      showDropdown: false,
      isDropdownOpen: false,
      questionText: '',
      answerText: '',
      answerPoints: null as number | null,
      showRoomCodeDropdown: false,
      roomCodeInput: null as number | null,
      joinRoomError: '',
    };
  },
  methods: {
    toggleSignUpMode() {
      this.signUpMode = true;
      this.loginMode = false;
      this.resetForm();
    },
    toggleLoginMode() {
      this.loginMode = true;
      this.signUpMode = false;
      this.resetForm();
    },
    async signUp() {
      try {
        const response = await axios.post('http://localhost:5180/Backend/Player', {
          username: this.usernameInput,
          password: this.passwordInput,
        });
        this.username = this.usernameInput;
        this.loggedIn = true;
        this.signUpMode = false;
        this.errorMessage = '';
      } catch (error) {
        this.errorMessage = 'Error signing up. Please try again.';
      }
    },
    async logIn() {
      try {
        const response = await axios.post('http://localhost:5180/Backend/Player/Authenticate', {
          username: this.usernameInput,
          password: this.passwordInput,
        });
        if (response.data) {
          const userStore = useUserStore();          
          userStore.setUser(response.data.id, response.data.username);
          this.username = response.data.username;
          this.userId = response.data.id;
          this.loggedIn = true;
          this.loginMode = false;
          this.errorMessage = '';
        } else {
          this.errorMessage = 'Invalid credentials';
        }
      } catch (error) {
        this.errorMessage = 'Error logging in. Please try again.';
      }
    },
    toggleDropdown() {
      this.showDropdown = !this.showDropdown;
    },
    logOut() {
      const userStore = useUserStore();
      userStore.logOut();
      this.loggedIn = false;
      this.username = '';
      this.userId = null;
      this.resetForm();
      this.showDropdown = false;
    },
    resetForm() {
      this.usernameInput = '';
      this.passwordInput = '';
      this.errorMessage = '';
    },
    startNewGame() {
      const userStore = useUserStore();
      if (userStore.userId === null) {
        console.error("User is not logged in.");
        return;
      }

      axios.post('http://localhost:5180/Backend/GameRoom', userStore.userId, {
        headers: {
          'Content-Type': 'application/json',
        },
      })
      .then(response => {
        console.log("Game room created:", response.data);
        this.$router.push({ name: 'LobbyPage' });
      })
      .catch(error => {
        console.error("Failed to start a new game:", error);
        this.errorMessage = 'Failed to start a new game. Please try again.';
      });
    },
    toggleQuestionDropdown() {
      this.isDropdownOpen = !this.isDropdownOpen;
    },
    async submitQuestion() {
      if (!this.questionText || !this.answerText || this.answerPoints === null) {
        this.isDropdownOpen = false;
        return;
      }

      try {
        const response = await axios.post('http://localhost:5180/Backend/Answer', {
          question: { questionText: this.questionText },
          answerText: this.answerText,
          answerPoints: this.answerPoints,
        });
        console.log('Question created successfully:', response.data);
        this.resetQuestionForm();
      } catch (error) {
        console.error('Failed to create question:', error);
      }
    },
    async joinRoom() {
    const userStore = useUserStore();
    if (!this.roomCodeInput) {
      this.joinRoomError = 'Please enter a room code.';
      return;
    }
    
    try {
      const response = await axios.post(
        `http://localhost:5180/Backend/GameRoom/JoinRoom`,
        { playerId: userStore.userId, roomCode: this.roomCodeInput },
        { headers: { 'Content-Type': 'application/json' } }
      );

      console.log("Joined game room:", response.data);
      this.$router.push({ name: 'LobbyPage' });
    } catch (error) {
      console.error("Failed to join the game room:", error);
      this.joinRoomError = 'Failed to join room. Please check the code and try again.';
    }
  },
    resetQuestionForm() {
      this.questionText = '';
      this.answerText = '';
      this.answerPoints = null;
      this.isDropdownOpen = false;
    },
    searchForGames() {
      this.showRoomCodeDropdown = !this.showRoomCodeDropdown;
    },
    browseQuestions() {
      this.$router.push({ name: 'BrowseQuestions' });
    },
  },
});
</script>



<style scoped>
.main-menu {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
  text-align: center;
}

.top-right {
  position: absolute;
  top: 10px;
  right: 10px;
}

.form-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-top: 20px;
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

.error {
  color: red;
  font-size: 14px;
}
</style>