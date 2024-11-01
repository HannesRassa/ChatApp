<template>
    <div class="main-menu">
      <h1>Main Menu</h1>
      <div class="menu-options">
        <button @click="startNewGame">Start New Game</button>
        <button @click="searchForGames">Search for Existing Games</button>
        <button @click="createQuestion">Create Question</button>
        <button @click="browseQuestions">Browse Questions</button>  
        <li><NuxtLink to ="/">Home</NuxtLink></li>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: "MainMenu",
    methods: {
      startNewGame() {
        // Logic to start a new game
        console.log("Starting a new game...");
        // this.$router.push({ name: 'NewGame' }); // Example route
      },
      searchForGames() {
        // Logic to search for existing games
        console.log("Searching for existing games...");
        // this.$router.push({ name: 'SearchGames' }); // Example route
      },
      createQuestion() {
        // Logic to create a new question
        console.log("Creating a new question...");
        // this.$router.push({ name: 'CreateQuestion' }); // Example route
      },
      browseQuestions() {
        // Logic to browse questions
        console.log("Browsing questions...");
        this.$router.push({ name: 'BrowseQuestions' });   
        
      }
    }
  };
  </script>
  
  <style scoped>
  .main-menu {
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
  