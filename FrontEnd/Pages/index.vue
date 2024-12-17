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
    <div v-if="showLobbyCodeDropdown" class="lobby-code-dropdown">
      <label for="lobbyCode">Enter Lobby Code:</label>
      <input type="number" id="lobbyCode" v-model="lobbyCodeInput" placeholder="e.g., 1234" required />
      <button @click="joinLobby">Join Lobby</button>
      <p v-if="joinLobbyError" class="error">{{ joinLobbyError }}</p>
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
.main-menu {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
  text-align: center;
  padding: 20px;
  background-color: #f4f0ff; /* Light lavender background for the layout */
}

.top-right {
  position: absolute;
  top: 10px;
  right: 10px;
  display: flex;
  gap: 10px;
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

.form-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
  background-color: #e0d4f7; /* Light purple background for the form */
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.form-container h2 {
  margin: 0;
  font-size: 24px;
  color: #5b2c6f; /* Darker purple for headings */
}

.form-container div {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 5px;
}

label {
  font-size: 14px;
  font-weight: bold;
  color: #5b2c6f;
}

input {
  padding: 8px;
  font-size: 14px;
  border: 1px solid #ccc;
  border-radius: 4px;
  width: 100%;
  box-sizing: border-box;
}

input:focus {
  border-color: #7d3c98;
  outline: none;
}

.menu-options {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 20px;
}

.menu-options button {
  background-color: #7d3c98;
  color: white;
  padding: 10px 30px;
  border-radius: 8px;
  font-size: 16px;
  transition: background-color 0.3s;
}

.menu-options button:hover {
  background-color: #5b2c6f;
}

.dropdown-menu {
  position: absolute;
  top: 40px;
  right: 10px;
  background-color: #e0d4f7; /* Light purple background */
  border: 1px solid #ccc;
  padding: 10px;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.dropdown-menu button {
  padding: 10px;
  font-size: 14px;
  background: none;
  border: none;
  text-align: left;
  width: 100%;
  color: #5b2c6f;
  cursor: pointer;
}

.dropdown-menu button:hover {
  background-color: #f4f0ff; /* Light lavender hover effect */
}

.error {
  color: red;
  font-size: 14px;
  margin-top: 10px;
}

.question-form {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 20px;
  background-color: #e0d4f7;
  padding: 20px;
  border-radius: 8px;
}

.question-form label {
  display: flex;
  flex-direction: column;
  font-size: 14px;
  font-weight: bold;
  color: #5b2c6f;
}

.lobby-code-dropdown {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 20px;
  align-items: center;
  background-color: #e0d4f7;
  padding: 20px;
  border-radius: 8px;
}

.lobby-code-dropdown label {
  font-size: 14px;
  font-weight: bold;
  color: #5b2c6f;
}

.lobby-code-dropdown input {
  padding: 8px;
  font-size: 14px;
  border: 1px solid #ccc;
  border-radius: 4px;
  width: 100%;
  box-sizing: border-box;
}

.lobby-code-dropdown button {
  background-color: #7d3c98;
  color: white;
  padding: 10px 20px;
  border-radius: 8px;
  font-size: 16px;
  transition: background-color 0.3s;
}

.lobby-code-dropdown button:hover {
  background-color: #5b2c6f;
}
</style>