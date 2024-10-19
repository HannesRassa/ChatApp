<template>
    <div>
      <button @click="redirectToChatPage">Go to Chat Page</button>
    </div>
  </template>
  
  <script>
  export default {
    methods: {
      redirectToChatPage() {
        this.$router.push({ name: 'ChatPage' }); // Use the route name 'ChatPage'
      }
    }
  };
  </script>
  
  <style scoped>
  button {
    padding: 10px 20px;
    font-size: 16px;
    cursor: pointer;
  }
  </style>
  