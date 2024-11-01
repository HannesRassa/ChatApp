<template>
  <div>
    <!-- Top Right Login/Sign Up Buttons or Username Display -->
    <div class="top-right">
      <div v-if="!loggedIn">
        <button @click="toggleSignUpMode">Sign Up</button>
        <button @click="toggleLoginMode">Log In</button>
      </div>
      <div v-else>
        <span @click="toggleUserOptions">{{ username }}</span>
        <div v-if="showUserOptions" class="dropdown">
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
        <button type="submit">{{ signUpMode ? "Sign Up" : "Log In" }}</button>
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
      </form>
    </div>

    <!-- Redirect Button for Logged In Users -->
    <button v-if="loggedIn" @click="redirectToChatPage">Go to Chat Page</button>
  </div>
</template>

<script>
import axios from 'axios';

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
      errorMessage: '', // New: for displaying error messages
      userId: null // New: to store user ID
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
    toggleUserOptions() {
      this.showUserOptions = !this.showUserOptions;
    },
    async signUp() {
      try {
        const newPlayer = {
          username: this.usernameInput,
          password: this.passwordInput,
        };
        const response = await axios.post('http://localhost:5180/Backend/Player', newPlayer);
        console.log('Player created successfully:', response.data);

        this.username = this.usernameInput;
        this.loggedIn = true;
        this.signUpMode = false;
        this.errorMessage = ''; // Clear error on success
      } catch (error) {
        this.errorMessage = 'Error signing up. Please try again.'; // Set error message
        console.error('Error signing up:', error);
      }
    },
    async logIn() {
      try {
        const response = await axios.post('http://localhost:5180/Backend/Player/Authenticate', {
          username: this.usernameInput,
          password: this.passwordInput,
        });

        if (response.data) {
          this.username = response.data.username;
          this.userId = response.data.id; // Store user ID
          this.loggedIn = true;
          this.loginMode = false;
          this.errorMessage = ''; // Clear error on success
        } else {
          this.errorMessage = 'Invalid credentials';
        }
      } catch (error) {
        this.errorMessage = 'Error logging in. Please try again.'; // Set error message
        console.error('Error logging in:', error);
      }
    },
    logOut() {
      this.loggedIn = false;
      this.username = '';
      this.userId = null; // Clear user ID
      this.resetForm();
    },
    resetForm() {
      this.usernameInput = '';
      this.passwordInput = '';
      this.errorMessage = ''; // Clear error when resetting
      this.showUserOptions = false;
    },
    redirectToChatPage() {
      this.$router.push({ name: 'ChatPage' });
    }
  }
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

