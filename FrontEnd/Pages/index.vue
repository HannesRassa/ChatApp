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
          <input
            type="password"
            id="password"
            v-model="passwordInput"
            required
          />
        </div>
        <button v-if="signUpMode" @click="signUp">Sign Up</button>
        <button v-if="loginMode" @click="logIn">Log In</button>
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
      </form>
    </div>

    <div v-if="loggedIn" class="menu-options">
      <button @click="startNewGame">Start New Game</button>
      <button @click="toggleLobbyCodeInput">Join Lobby</button>
      <button @click="searchForGames">Search for Existing Games</button>
      <button @click="createQuestions">Create QPacks</button>
      <button @click="browseQuestions">Browse QPacks</button>
    </div>

    <div v-if="showLobbyCodeInput" class="lobby-code-input">
      <input
      type="text"
      id="lobbyCode"
      class="form__field"
      v-model="exactLobbyCodeInput"
      placeholder="e.g., 1234"
      required
      />
      <label for="lobbyCode" class="form__label">Enter Room Code:</label>
      <button @click="joinExactLobby()">Join Lobby</button>
      <p v-if="joinLobbyError" class="error">{{ joinLobbyError }}</p>
    </div>

    <div v-if="showLobbyCodeDropdown" class="lobby-code-dropdown">
      <label for="lobbyCode">Enter Lobby Code:</label>
      <input
        type="number"
        id="lobbyCode"
        v-model="exactLobbyCodeInput"
        placeholder="e.g., 1234"
        required
      />
      <button @click="joinLobby">Join Lobby</button>
      <p v-if="joinLobbyError" class="error">{{ joinLobbyError }}</p>
    </div>
  </div>
</template>

<script lang="ts">
import axios from "axios";
import { defineComponent } from "vue";
import { useUserStore } from "@/stores/userStore";
import axiosInstance, { isTokenExpired } from "~/utils/axiosInstance";

export default defineComponent({
  data() {
    return {
      usernameInput: "",
      passwordInput: "",
      username: "",
      loggedIn: false,
      signUpMode: false,
      loginMode: false,
      errorMessage: "",
      userId: null as number | null,
      showDropdown: false,
      isDropdownOpen: false,
      questionText: "",
      answerText: "",
      answerPoints: null as number | null,
      showLobbyCodeDropdown: false,
      lobbyCodeInput: null as number | null,
      exactLobbyCodeInput: null as number | null,
      joinLobbyError: "",
      showLobbyCodeInput: false,
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
        const response = await axiosInstance.post("Player", {
          username: this.usernameInput,
          password: this.passwordInput,
        });

        const userStore = useUserStore();

        // Update user state with playerId
        userStore.setUser(
          response.data.id,
          response.data.username,
          response.data.token,
          response.data.expiration
        );

        this.username = userStore.username;
        this.userId = null;
        this.loggedIn = true;
        this.signUpMode = false;
        this.errorMessage = "";
        console.log(this.userId);
      } catch (error) {
        this.errorMessage = "Error signing up. Please try again.";
        console.error("Sign-up error:", error);
      }
      try {
        const response = await axiosInstance.post("Player/Authenticate", {
          username: this.usernameInput,
          password: this.passwordInput,
        });
        if (response.data) {
          const userStore = useUserStore();
          // Update user state with playerId
          userStore.setUser(
            response.data.id,
            this.usernameInput,
            response.data.token,
            response.data.expiration
          );

          this.username = userStore.username;
          this.userId = userStore.userId;
          this.loggedIn = true;
          this.loginMode = false;
          this.errorMessage = "";
        } else {
          this.errorMessage = "Invalid credentials";
        }
      } catch (error) {
        this.errorMessage = "Error logging in. Please try again.";
        console.error("Login error:", error);
      }
    },
    async logIn() {
      try {
        const response = await axiosInstance.post("Player/Authenticate", {
          username: this.usernameInput,
          password: this.passwordInput,
        });
        if (response.data) {
          const userStore = useUserStore();
          // Update user state with playerId
          userStore.setUser(
            response.data.id,
            this.usernameInput,
            response.data.token,
            response.data.expiration
          );

          this.username = userStore.username;
          this.userId = userStore.userId;
          this.loggedIn = true;
          this.loginMode = false;
          this.errorMessage = "";
        } else {
          this.errorMessage = "Invalid credentials";
        }
      } catch (error) {
        this.errorMessage = "Error logging in. Please try again.";
        console.error("Login error:", error);
      }
    },
    toggleDropdown() {
      this.showDropdown = !this.showDropdown;
    },
    logOut() {
      const userStore = useUserStore();
      userStore.logOut();
      this.loggedIn = false;
      this.username = "";
      this.userId = null;
      this.resetForm();
      this.showDropdown = false;
    },
    resetForm() {
      this.usernameInput = "";
      this.passwordInput = "";
      this.errorMessage = "";
    },

    startNewGame() {
      const userStore = useUserStore();
      if (userStore.userId === null) {
        console.error("User is not logged in.");
        return;
      }

      axiosInstance
        .post("https://localhost:7269/Backend/Lobby", userStore.userId, {
          headers: {
            "Content-Type": "application/json",
          },
        })
        .then((response) => {
          console.log("Game lobby created:", response.data);
          this.$router.push({ name: "Lobby" });
        })
        .catch((error) => {
          console.error("Failed to start a new game:", error);
          this.errorMessage = "Failed to start a new game. Please try again.";
        });
    },
    createQuestions() {
      this.$router.push({ name: "CreateQuestions" });
    },

    toggleLobbyCodeInput() {
      this.showLobbyCodeInput = !this.showLobbyCodeInput;
      //this.$router.push({ name: "GameRooms" });
    },

    async joinExactLobby() {
      const userStore = useUserStore;
      if (
        !this.exactLobbyCodeInput ||
        this.exactLobbyCodeInput.toString().trim() === ""
      ) {
        this.joinLobbyError = "Room code cannot be empty.";
        console.log("this.exactLobbyCodeInput is empty");
        return;
      }
      try {
        console.log("Joining lobby with code:", this.exactLobbyCodeInput);
        const requsest = {
          playerId: this.userId,
          roomCode: this.exactLobbyCodeInput,
        };
        console.log(`requst joinlobby log : ${JSON.stringify(requsest,null,2)}`);
        await axios.post("https://localhost:7269/Backend/Lobby/JoinLobby", requsest);
        this.$router.push({ name: "Lobby" });
      } catch (error) {
        console.error("Failed to join the game lobby:", error);
        this.joinLobbyError =
          "Failed to join lobby. Please check the code and try again.";
      }
    },
    async joinLobby() {
      const userStore = useUserStore();
      if (!this.lobbyCodeInput) {
        this.joinLobbyError = "Please enter a lobby code.";
        return;
      }

      try {
        const response = await axios.post(
          `https://localhost:7269/Backend/Lobby/JoinLobby`,
          { playerId: userStore.userId, lobbyCode: this.lobbyCodeInput },
          { headers: { "Content-Type": "application/json" } }
        );

        console.log("Joined game lobby:", response.data);
        this.$router.push({ name: "LobbyPage" });
      } catch (error) {
        console.error("Failed to join the game lobby:", error);
        this.joinLobbyError =
          "Failed to join lobby. Please check the code and try again.";
      }
    },
    resetQuestionForm() {
      this.questionText = "";
      this.answerText = "";
      this.answerPoints = null;
      this.isDropdownOpen = false;
    },
    searchForGames() {
      //this.showLobbyCodeDropdown = !this.showLobbyCodeDropdown;
      //this.$router.push(`Lobbies`);
      this.$router.push({ name: "GameRooms" });
    },
    browseQuestions() {
      this.$router.push({ name: "QuestionPacks" });
    },
  },
  mounted() {
    const userStore = useUserStore();
    userStore.loadUser();
    if (userStore.token && !isTokenExpired(userStore.tokenExpiration)) {
      this.loggedIn = true;
      this.username = userStore.username;
      this.userId = userStore.userId;
    } else {
      userStore.logOut();
      this.loggedIn = false;
      this.username = "";
      this.userId = null;
      console.warn("Session expired. Please log in again.");
    }
  },
});
</script>

<style scoped>
@import url("https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap");

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: "Poppins", sans-serif;
}
.lobby-code-input {
  position: relative;
  margin-top: 20px;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 10px;
  background: #ffffff;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
  max-width: 400px;
  width: 100%;
}
.main-menu {
  background: linear-gradient(to bottom right, #2c3e50, #4ca1af);
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
  background: linear-gradient(135deg, #2c3e50, #4ca1af);
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
  background: #4ca1af;
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
  color: #4ca1af;
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
  border-color: #4ca1af;
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

.form__label {
  position: absolute;
  top: 0;
  display: block;
  transition: 0.2s;
  font-size: 17px;
  color: #000;
  pointer-events: none;
}

.form__field {
  font-family: inherit;
  width: 100%;
  border: none;
  border-bottom: 2px solid #9b9b9b;
  outline: 0;
  font-size: 16px;
  color: #000;
  padding: 7px 0;
  background: transparent;
  transition: border-color 0.2s;
}

.form__field::placeholder {
  color: transparent;
}

.form__field:placeholder-shown ~ .form__label {
  font-size: 15px;
  cursor: text;
  top: 20px;
}

.form__field:focus {
  padding-bottom: 6px;
  font-weight: 700;
  border-width: 3px;
  border-image: linear-gradient(to right, #116399, #38caef);
  border-image-slice: 1;
}

.form__field:focus ~ .form__label {
  position: absolute;
  top: 0;
  display: block;
  transition: 0.2s;
  font-size: 17px;
  color: #38caef;
  font-weight: 700;
}

/* reset input */
.form__field:required, .form__field:invalid {
  box-shadow: none;
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
