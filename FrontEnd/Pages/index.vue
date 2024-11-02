<template>
  <div>
    <!-- Top Right Login/Sign Up Buttons or Username Display -->
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

    <!-- Sign Up / Login Form in the Center -->
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

    <!-- Redirect Button for Logged In Users -->
    <button v-if="loggedIn" @click="redirectToChatPage">Go to Chat Page</button>
  </div>
</template>

<script lang="ts">
import axios from 'axios';
import { useUserStore } from '~/stores/userStore';

export default {
  data() {
    return {
      usernameInput: '',
      passwordInput: '',
      username: '',
      loggedIn: false,
      signUpMode: false,
      loginMode: false,
      showUserOptions: false,
      errorMessage: '',
      userId: null,
      showDropdown: false,
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
        const newPlayer = {
          username: this.usernameInput,
          password: this.passwordInput,
        };

        const response = await axios.post('http://localhost:5180/Backend/Player', newPlayer);
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
        const loginData = {
          username: this.usernameInput,
          password: this.passwordInput,
        };

        const response = await axios.post('http://localhost:5180/Backend/Player/Authenticate', loginData);

        if (response.data) {
          const userStore = useUserStore();
          userStore.setUser(response.data.id, response.data.username);  // Store user ID and username
          
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
      userStore.logOut();  // Clear user ID and username

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
      this.showUserOptions = false;
    },
    redirectToChatPage() {
      this.$router.push({ name: 'MainMenu' });
    },
  },
};
</script>

<style scoped>
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

button {
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
}

.dropdown {
  display: flex;
  flex-direction: column;
}

form div {
  margin-bottom: 10px;
}

.error {
  color: red;
  font-size: 14px;
}
</style>
