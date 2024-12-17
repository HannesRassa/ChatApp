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

    <!-- Login/Sign Up Form -->
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
        <button>{{ signUpMode ? "Sign Up" : "Log In" }}</button>
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
      </form>
    </div>

    <!-- Menu Options -->
    <div v-if="loggedIn" class="menu-options">
      <button @click="startNewGame">Start New Game</button>
      <button @click="searchForGames">Search for Games</button>
      <button @click="toggleQuestionDropdown">Create Question</button>
      <button @click="browseQuestions">Browse Questions</button>
    </div>

    <!-- Question Form -->
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
  </div>
</template>



<script lang="ts">
import axios from 'axios';
import { defineComponent } from 'vue';
import { useUserStore } from '@/stores/userStore';
import axiosInstance, { isTokenExpired } from '~/utils/axiosInstance';

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
      userId: null as number | null,
      showDropdown: false,
      isDropdownOpen: false,
      questionText: '',
      answerText: '',
      answerPoints: null as number | null,
      showLobbyCodeDropdown: false,
      lobbyCodeInput: null as number | null,
      joinLobbyError: '',
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
        const response = await axiosInstance.post('Player', {
          username: this.usernameInput,
          password: this.passwordInput,
        });

        const userStore = useUserStore();

        // Update user state with playerId
        userStore.setUser(response.data.id, response.data.username, response.data.token, response.data.expiration);

        this.username = userStore.username;
        this.userId = null;
        this.loggedIn = true;
        this.signUpMode = false;
        this.errorMessage = '';
        console.log(this.userId);
      } catch (error) {
        this.errorMessage = 'Error signing up. Please try again.';
        console.error('Sign-up error:', error);
      }
      try {
        const response = await axiosInstance.post('Player/Authenticate', {
          username: this.usernameInput,
          password: this.passwordInput,
        });
        if (response.data) {
          const userStore = useUserStore();
          // Update user state with playerId
          userStore.setUser(response.data.id, this.usernameInput, response.data.token, response.data.expiration);

          this.username = userStore.username;
          this.userId = userStore.userId;
          this.loggedIn = true;
          this.loginMode = false;
          this.errorMessage = '';
        } else {
          this.errorMessage = 'Invalid credentials';
        }
      } catch (error) {
        this.errorMessage = 'Error logging in. Please try again.';
        console.error('Login error:', error);
      }
    },
    async logIn() {
      try {
        const response = await axiosInstance.post('Player/Authenticate', {
          username: this.usernameInput,
          password: this.passwordInput,
        });
        if (response.data) {
          const userStore = useUserStore();
          // Update user state with playerId
          userStore.setUser(response.data.id, this.usernameInput, response.data.token, response.data.expiration);

          this.username = userStore.username;
          this.userId = userStore.userId;
          this.loggedIn = true;
          this.loginMode = false;
          this.errorMessage = '';
        } else {
          this.errorMessage = 'Invalid credentials';
        }
      } catch (error) {
        this.errorMessage = 'Error logging in. Please try again.';
        console.error('Login error:', error);
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

      axios.post('http://localhost:5180/Backend/Lobby', userStore.userId, {
        headers: {
          'Content-Type': 'application/json',
        },
      })
      .then(response => {
        console.log("Game lobby created:", response.data);
        this.$router.push({ name: 'Lobby' });
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
    async joinLobby() {
      const userStore = useUserStore();
      if (!this.lobbyCodeInput) {
        this.joinLobbyError = 'Please enter a lobby code.';
        return;
      }
      
      try {
        const response = await axios.post(
          `http://localhost:5180/Backend/Lobby/JoinLobby`,
          { playerId: userStore.userId, lobbyCode: this.lobbyCodeInput },
          { headers: { 'Content-Type': 'application/json' } }
        );

        console.log("Joined game lobby:", response.data);
        this.$router.push({ name: 'LobbyPage' });
      } catch (error) {
        console.error("Failed to join the game lobby:", error);
        this.joinLobbyError = 'Failed to join lobby. Please check the code and try again.';
      }
    },
    resetQuestionForm() {
      this.questionText = '';
      this.answerText = '';
      this.answerPoints = null;
      this.isDropdownOpen = false;
    },
    searchForGames() {
      //this.showLobbyCodeDropdown = !this.showLobbyCodeDropdown;
      this.$router.push(`Lobbies`)
    },
    browseQuestions() {
      this.$router.push({ name: 'BrowseQuestions' });
    },
  },
  mounted() {
    const userStore = useUserStore();
    userStore.loadUser();
    if (userStore.token && !isTokenExpired(userStore.tokenExpiration)) {
      this.loggedIn = true;
      this.username = userStore.username;
    } else {
      userStore.logOut();
      this.loggedIn = false;
      this.username = '';
      console.warn('Session expired. Please log in again.');
    }

  },
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap');

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: 'Poppins', sans-serif;
}

.main-menu {
  background: linear-gradient(to bottom right, #f8f9fd, #e9e4f5);
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.top-right {
  position: absolute;
  top: 20px;
  right: 20px;
  display: flex;
  gap: 15px;
}

button {
  background: linear-gradient(135deg, #6c5ce7, #a29bfe);
  border: none;
  padding: 12px 24px;
  border-radius: 30px;
  color: #fff;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
}

button:hover {
  background: #5c51c6;
  transform: translateY(-2px);
}

.form-container {
  background: #ffffff;
  padding: 30px;
  border-radius: 16px;
  box-shadow: 0px 6px 20px rgba(0, 0, 0, 0.1);
  max-width: 400px;
  width: 100%;
  margin: 20px 0;
  text-align: center;
}

.form-container h2 {
  margin-bottom: 20px;
  color: #6c5ce7;
}

.form-container label {
  display: block;
  text-align: left;
  margin-bottom: 8px;
  font-weight: 600;
  color: #333;
}

.form-container input {
  width: 100%;
  padding: 10px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  margin-bottom: 15px;
  transition: border 0.3s;
}

.form-container input:focus {
  border-color: #6c5ce7;
  outline: none;
}

.menu-options {
  margin-top: 20px;
  display: flex;
  flex-direction: column;
  gap: 15px;
  align-items: center;
}

.menu-options button {
  width: 220px;
}

.dropdown-menu {
  background: white;
  border: 1px solid #ddd;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
  padding: 10px;
  border-radius: 8px;
  position: absolute;
  top: 50px;
  right: 0;
}

.question-form,
.lobby-code-dropdown {
  background: #ffffff;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
  margin-top: 20px;
  width: 100%;
  max-width: 400px;
}

.question-form label {
  margin-bottom: 8px;
}

.error {
  color: #ff4757;
  margin-top: 10px;
  font-size: 14px;
}
</style>