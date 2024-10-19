<template>
    <div>
      <button @click="startCountdown">Start Countdown</button>
      <div v-if="countdownText">{{ countdownText }}</div>
    </div>
  </template>
  
  <script>

  import axios from 'axios'; 
  
  export default {
    data() {
      return {
        countdownSteps: ["Ready", "Steady", "Go!"],
        countdownIndex: -1,
        countdownText: "",
      };
    },
    methods: {
      startCountdown() {
        this.countdownIndex = 0;
        this.updateCountdown();
      },
      updateCountdown() {
        if (this.countdownIndex < this.countdownSteps.length) {
          this.countdownText = this.countdownSteps[this.countdownIndex];
          setTimeout(() => {
            this.countdownIndex++;
            this.updateCountdown();
          }, 1000); // 1-second delay between each step
        } else {
          this.countdownText = "";
          this.startGame(); // Call backend API to start the game after countdown
        }
      },
      startGame() {
        // Replace the URL with your backend endpoint
        axios
          .post('http://localhost:5180/Backend/Game/Start')
          .then(response => {
            console.log('Game started successfully!', response.data);
            // Handle success (optional)
          })
          .catch(error => {
            console.error('Error starting game:', error);
            // Handle error
          });
      },
    },
  };
  </script>
  
  <style scoped>
  button {
    margin-bottom: 20px;
  }
  
  div {
    font-size: 2rem;
    font-weight: bold;
  }
  </style>
  